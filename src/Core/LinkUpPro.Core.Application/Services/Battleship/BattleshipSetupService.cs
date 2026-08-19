using AutoMapper;
using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Exceptions;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Domain.Rules.Battleship.Ship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipSetupService : IBattleshipSetupService
{
    private readonly IBattleshipGameRepository _gameRepo;
    private readonly IBattleshipBoardRepository _boardRepo;
    private readonly IBattleshipShipRepository _shipRepo;
    private readonly IMapper _mapper;

    public BattleshipSetupService(
        IBattleshipGameRepository gameRepo,
        IBattleshipBoardRepository boardRepo,
        IBattleshipShipRepository shipRepo,
        IMapper mapper)
    {
        _gameRepo = gameRepo;
        _boardRepo = boardRepo;
        _shipRepo = shipRepo;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<ShipDto>> PlaceShipAsync(PlaceShipDto dto)
    {
        var board = await _boardRepo.GetByGameAndOwnerAsync(dto.GameId, dto.PlayerId);
        if (board == null)
            return ServiceResponse<ShipDto>.Failure("Tablero no encontrado.");

        var ships = await _shipRepo.GetByBoardAsync(board.Id);

        // Buscar un barco sin colocar del tamaño indicado (coordenadas -1 = no posicionado)
        var ship = ships.FirstOrDefault(s => (int)s.Size == (int)dto.Size && s.StartCoordinateX == -1 && s.StartCoordinateY == -1);
        if (ship == null)
            return ServiceResponse<ShipDto>.Failure("Barco no encontrado para ese tamaño o ya fue posicionado.");

        // Validar colision
        var occupiedCells = await _shipRepo.GetOccupiedCellsAsync(board.Id);
        var newCells = CalculateCells(dto.StartY, dto.StartX, (int)dto.Size, dto.Direction);

        // Verificar limites 12x12
        bool outOfBounds = dto.Direction switch
        {
            ShipDirection.Right => dto.StartX + (int)dto.Size > 12,
            ShipDirection.Left => dto.StartX - (int)dto.Size + 1 < 0,
            ShipDirection.Down => dto.StartY + (int)dto.Size > 12,
            ShipDirection.Up => dto.StartY - (int)dto.Size + 1 < 0,
            _ => true
        };

        if (outOfBounds)
        {
            var dirSpanish = dto.Direction switch
            {
                ShipDirection.Right => "derecha",
                ShipDirection.Left  => "izquierda",
                ShipDirection.Down  => "abajo",
                ShipDirection.Up    => "arriba",
                _                   => dto.Direction.ToString()
            };
            return ServiceResponse<ShipDto>.Failure($"El barco sale del tablero hacia {dirSpanish}.");
        }

        // Verificar colisiones (excluir el barco actual si ya estaba colocado)
        var cellsWithoutCurrentShip = occupiedCells.ToList();
        var currentShipCells = CalculateCells(ship.StartCoordinateY, ship.StartCoordinateX, (int)ship.Size, ship.Direction);
        cellsWithoutCurrentShip = cellsWithoutCurrentShip
            .Where(c => !currentShipCells.Any(sc => sc.Row == c.Row && sc.Col == c.Col))
            .ToList();

        if (newCells.Any(nc => cellsWithoutCurrentShip.Any(oc => oc.Row == nc.Row && oc.Col == nc.Col)))
            return ServiceResponse<ShipDto>.Failure("El barco se superpone con otro barco.");

        ship.StartCoordinateX = dto.StartX;
        ship.StartCoordinateY = dto.StartY;
        ship.Direction = dto.Direction;
        await _shipRepo.UpdateAsync(ship);

        return ServiceResponse<ShipDto>.Success(_mapper.Map<ShipDto>(ship));
    }

    public async Task<ServiceResponse<BattleshipBoardDto>> GetBoardAsync(Guid gameId, Guid playerId)
    {
        var board = await _boardRepo.GetByGameAndOwnerAsync(gameId, playerId);
        if (board == null)
            return ServiceResponse<BattleshipBoardDto>.Failure("Tablero no encontrado.");

        var ships = await _shipRepo.GetByBoardAsync(board.Id);
        var attacks = board.ReceivedAttacks ?? new List<BattleshipAttack>();

        var dto = _mapper.Map<BattleshipBoardDto>(board);
        dto.Ships = _mapper.Map<List<ShipDto>>(ships);
        dto.ReceivedAttacks = _mapper.Map<List<AttackResultDto>>(attacks);

        return ServiceResponse<BattleshipBoardDto>.Success(dto);
    }

    public async Task<bool> BothPlayersReadyAsync(Guid gameId)
        => await _boardRepo.BothPlayersReadyAsync(gameId);

    public async Task<BaseResult> ConfirmSetupAsync(Guid gameId, Guid playerId)
    {
        var board = await _boardRepo.GetByGameAndOwnerAsync(gameId, playerId);
        if (board == null)
            return BaseResult.Fail("Tablero no encontrado.");

        var ships = await _shipRepo.GetByBoardAsync(board.Id);
        var placedShips = ships.Where(s => s.StartCoordinateX >= 0 && s.StartCoordinateY >= 0).ToList();
        if (placedShips.Count < 5)
            return BaseResult.Fail("Debes colocar los 5 barcos antes de confirmar.");

        // Verificar si el oponente ya confirmó (ambos tienen 5 barcos)
        var bothReady = await _boardRepo.BothPlayersReadyAsync(gameId);
        if (bothReady)
        {
            var game = await _gameRepo.GetByIdAsync(gameId);
            if (game != null && game.Status == GameStatus.PlacingShips)
            {
                game.Status = GameStatus.InProgress;
                game.CurrentTurnPlayerId = game.Player1Id;
                game.TurnStartedAt = DateTime.UtcNow;
                await _gameRepo.UpdateAsync(game);
            }
        }

        return BaseResult.Ok();
    }

    private static IReadOnlyList<(int Row, int Col)> CalculateCells(int startRow, int startCol, int size, ShipDirection direction)
    {
        var cells = new List<(int, int)>();
        for (int i = 0; i < size; i++)
        {
            var (row, col) = direction switch
            {
                ShipDirection.Right => (startRow, startCol + i),
                ShipDirection.Left => (startRow, startCol - i),
                ShipDirection.Down => (startRow + i, startCol),
                ShipDirection.Up => (startRow - i, startCol),
                _ => (startRow, startCol)
            };
            cells.Add((row, col));
        }
        return cells;
    }
}
