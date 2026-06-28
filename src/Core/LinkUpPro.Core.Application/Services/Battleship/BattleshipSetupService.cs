using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipSetupService : IBattleshipSetupService
{
    private readonly IBattleshipGameRepository _gameRepository;
    private readonly IBattleshipBoardRepository _boardRepository;
    private readonly IBattleshipShipRepository _shipRepository;

    public BattleshipSetupService(
        IBattleshipGameRepository gameRepository,
        IBattleshipBoardRepository boardRepository,
        IBattleshipShipRepository shipRepository)
    {
        _gameRepository = gameRepository;
        _boardRepository = boardRepository;
        _shipRepository = shipRepository;
    }

    public async Task<ServiceResponse<BattleshipBoardDto>> GetBoardAsync(Guid gameId, Guid playerId)
    {
        var board = await _boardRepository.Query()
            .Include(b => b.Ships)
            .Include(b => b.ReceivedAttacks)
            .FirstOrDefaultAsync(b => b.GameId == gameId && b.PlayerId == playerId);

        if (board == null)
            return ServiceResponse<BattleshipBoardDto>.Failure("Tablero no encontrado.");

        var dto = new BattleshipBoardDto
        {
            Id = board.Id,
            GameId = board.GameId,
            PlayerId = board.PlayerId,
            Ships = board.Ships.Select(s => new ShipDto
            {
                Id = s.Id,
                Size = s.Size,
                Direction = s.Direction,
                StartX = s.StartCoordinateX,
                StartY = s.StartCoordinateY,
                IsSunk = s.IsSunk
            }).ToList(),
            ReceivedAttacks = board.ReceivedAttacks.Select(a => new AttackResultDto
            {
                CoordinateX = a.CoordinateX,
                CoordinateY = a.CoordinateY,
                IsHit = a.IsHit,
                IsSunk = false // Simplificado para DTO
            }).ToList()
        };

        return ServiceResponse<BattleshipBoardDto>.Success(dto);
    }

    public async Task<ServiceResponse<ShipDto>> PlaceShipAsync(PlaceShipDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(dto.GameId);
        if (game == null || game.Status != GameStatus.PlacingShips)
            return ServiceResponse<ShipDto>.Failure("Juego no válido o no está en fase de colocación.");

        var board = await _boardRepository.Query()
            .Include(b => b.Ships)
            .FirstOrDefaultAsync(b => b.GameId == dto.GameId && b.PlayerId == dto.PlayerId);

        if (board == null)
        {
            board = new Domain.Entities.Battleship.BattleshipBoard
            {
                GameId = dto.GameId,
                PlayerId = dto.PlayerId
            };
            await _boardRepository.AddAsync(board);
        }

        // Lógica súper básica de validación (debería comprobar colisiones y límites de tablero)
        int length = (int)dto.Size;
        if (dto.Direction == ShipDirection.Horizontal && dto.StartX + length > 10)
            return ServiceResponse<ShipDto>.Failure("El barco se sale del tablero horizontalmente.");
        if (dto.Direction == ShipDirection.Vertical && dto.StartY + length > 10)
            return ServiceResponse<ShipDto>.Failure("El barco se sale del tablero verticalmente.");

        // TODO: Comprobar colisión con otros barcos de board.Ships...

        var ship = new Domain.Entities.Battleship.BattleshipShip
        {
            BoardId = board.Id,
            Size = dto.Size,
            Direction = dto.Direction,
            StartCoordinateX = dto.StartX,
            StartCoordinateY = dto.StartY,
            IsSunk = false
        };

        await _shipRepository.AddAsync(ship);

        return ServiceResponse<ShipDto>.Success(new ShipDto
        {
            Id = ship.Id,
            Size = ship.Size,
            Direction = ship.Direction,
            StartX = ship.StartCoordinateX,
            StartY = ship.StartCoordinateY,
            IsSunk = ship.IsSunk
        });
    }

    public async Task<BaseResult> ConfirmSetupAsync(Guid gameId, Guid playerId)
    {
        var game = await _gameRepository.Query()
            .Include(g => g.Boards).ThenInclude(b => b.Ships)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return BaseResult.Fail("Juego no encontrado.");

        var playerBoard = game.Boards.FirstOrDefault(b => b.PlayerId == playerId);
        if (playerBoard == null || playerBoard.Ships.Count < 5) // Asumiendo 5 barcos como estándar
            return BaseResult.Fail("Aún no has colocado todos tus barcos.");

        // Si ambos jugadores tienen 5 barcos (ya confirmaron)
        if (game.Boards.Count == 2 && game.Boards.All(b => b.Ships.Count == 5))
        {
            game.Status = GameStatus.InProgress;
            await _gameRepository.UpdateAsync(game);
        }

        return BaseResult.Ok();
    }
}
