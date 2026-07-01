using AutoMapper;
using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Exceptions;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Domain.Rules.Battleship.Attack;
using LinkUpPro.Domain.Rules.Battleship.Game;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipAttackService : IBattleshipAttackService
{
    private readonly IBattleshipGameRepository _gameRepo;
    private readonly IBattleshipBoardRepository _boardRepo;
    private readonly IBattleshipShipRepository _shipRepo;
    private readonly IBattleshipAttackRepository _attackRepo;
    private readonly IMapper _mapper;

    public BattleshipAttackService(
        IBattleshipGameRepository gameRepo,
        IBattleshipBoardRepository boardRepo,
        IBattleshipShipRepository shipRepo,
        IBattleshipAttackRepository attackRepo,
        IMapper mapper)
    {
        _gameRepo = gameRepo;
        _boardRepo = boardRepo;
        _shipRepo = shipRepo;
        _attackRepo = attackRepo;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<BattleshipBoardDto>> GetOpponentBoardAsync(Guid gameId, Guid currentUserId)
    {
        var game = await _gameRepo.GetWithBoardsAsync(gameId);
        if (game == null)
            return ServiceResponse<BattleshipBoardDto>.Failure("La partida no existe.");

        var opponentId = currentUserId == game.Player1Id ? game.Player2Id : game.Player1Id;

        var opponentBoard = await _boardRepo.GetByGameAndOwnerAsync(gameId, opponentId);
        if (opponentBoard == null)
            return ServiceResponse<BattleshipBoardDto>.Failure("Tablero del oponente no encontrado.");

        var dto = _mapper.Map<BattleshipBoardDto>(opponentBoard);
        dto.Ships = _mapper.Map<List<ShipDto>>(opponentBoard.Ships ?? new List<BattleshipShip>());
        dto.ReceivedAttacks = _mapper.Map<List<AttackResultDto>>(opponentBoard.ReceivedAttacks ?? new List<BattleshipAttack>());

        return ServiceResponse<BattleshipBoardDto>.Success(dto);
    }

    public async Task<ServiceResponse<AttackResultDto>> AttackAsync(Guid gameId, Guid attackerId, int row, int col)
    {
        var game = await _gameRepo.GetWithBoardsAsync(gameId);
        if (game == null)
            return ServiceResponse<AttackResultDto>.Failure("La partida no existe.");

        // Verificar timeout 48h
        if (game.TurnStartedAt.HasValue &&
            DateTime.UtcNow - game.TurnStartedAt.Value > TimeSpan.FromHours(48))
        {
            var timeoutWinnerId = game.CurrentTurnPlayerId == game.Player1Id
                ? game.Player2Id
                : game.Player1Id;

            game.Status = GameStatus.Finished;
            game.WinnerId = timeoutWinnerId;
            await _gameRepo.UpdateAsync(game);

            return ServiceResponse<AttackResultDto>.Failure("La partida finalizo por inactividad.");
        }

        // Validar reglas
        try
        {
            RuleValidator.CheckRule(new GameMustBeActiveToAttackRule(game.Status));
            RuleValidator.CheckRule(new AttackerMustHaveActiveTurnRule(game.CurrentTurnPlayerId, attackerId));
        }
        catch (Exception ex)
        {
            return ServiceResponse<AttackResultDto>.Failure(ex.Message);
        }

        var alreadyAttacked = await _attackRepo.HasAttackedCellAsync(gameId, attackerId, row, col);
        try { RuleValidator.CheckRule(new CellMustNotBeAlreadyAttackedRule(alreadyAttacked)); }
        catch (Exception ex) { return ServiceResponse<AttackResultDto>.Failure(ex.Message); }

        // Determinar hit o miss
        var opponentId = attackerId == game.Player1Id ? game.Player2Id : game.Player1Id;

        var opponentBoard = await _boardRepo.GetByGameAndOwnerAsync(gameId, opponentId);
        if (opponentBoard == null)
            return ServiceResponse<AttackResultDto>.Failure("Tablero del oponente no encontrado.");

        var occupiedCells = await _shipRepo.GetOccupiedCellsAsync(opponentBoard.Id);
        var isHit = occupiedCells.Any(c => c.Row == row && c.Col == col);

        // Persistir ataque
        await _attackRepo.AddAsync(new BattleshipAttack
        {
            Id = Guid.NewGuid(),
            BoardId = opponentBoard.Id,
            CoordinateX = col,
            CoordinateY = row,
            IsHit = isHit
        });

        // Verificar si gano
        var allSunk = await _attackRepo.AllShipsSunkAsync(gameId, attackerId, occupiedCells);

        if (allSunk)
        {
            game.Status = GameStatus.Finished;
            game.WinnerId = attackerId;
            game.Result = GameResult.None;
            await _gameRepo.UpdateAsync(game);

            return ServiceResponse<AttackResultDto>.Success(new AttackResultDto
            {
                CoordinateX = col,
                CoordinateY = row,
                IsHit = true,
                IsSunk = true,
                IsGameOver = true,
                WinnerId = attackerId
            });
        }

        // Cambiar turno
        game.CurrentTurnPlayerId = opponentId;
        game.TurnStartedAt = DateTime.UtcNow;
        await _gameRepo.UpdateAsync(game);

        return ServiceResponse<AttackResultDto>.Success(new AttackResultDto
        {
            CoordinateX = col,
            CoordinateY = row,
            IsHit = isHit,
            IsSunk = false,
            IsGameOver = false
        });
    }
}
