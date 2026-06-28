using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipAttackService : IBattleshipAttackService
{
    private readonly IBattleshipGameRepository _gameRepository;
    private readonly IBattleshipBoardRepository _boardRepository;
    private readonly IBattleshipAttackRepository _attackRepository;
    private readonly IBattleshipShipRepository _shipRepository;
    private readonly INotificationDispatchService _notificationDispatch;

    public BattleshipAttackService(
        IBattleshipGameRepository gameRepository,
        IBattleshipBoardRepository boardRepository,
        IBattleshipAttackRepository attackRepository,
        IBattleshipShipRepository shipRepository,
        INotificationDispatchService notificationDispatch)
    {
        _gameRepository = gameRepository;
        _boardRepository = boardRepository;
        _attackRepository = attackRepository;
        _shipRepository = shipRepository;
        _notificationDispatch = notificationDispatch;
    }

    public async Task<ServiceResponse<AttackResultDto>> AttackAsync(AttackDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(dto.GameId);
        if (game == null || game.Status != GameStatus.InProgress)
            return ServiceResponse<AttackResultDto>.Failure("Juego no válido o no está en progreso.");

        if (game.CurrentTurnPlayerId != dto.AttackerPlayerId)
            return ServiceResponse<AttackResultDto>.Failure("No es tu turno.");

        var opponentId = game.Player1Id == dto.AttackerPlayerId ? game.Player2Id : game.Player1Id;

        var opponentBoard = await _boardRepository.Query()
            .Include(b => b.Ships)
            .Include(b => b.ReceivedAttacks)
            .FirstOrDefaultAsync(b => b.GameId == dto.GameId && b.PlayerId == opponentId);

        if (opponentBoard == null)
            return ServiceResponse<AttackResultDto>.Failure("Tablero del oponente no encontrado.");

        // Verificar si ya atacó ahí
        if (opponentBoard.ReceivedAttacks.Any(a => a.CoordinateX == dto.TargetX && a.CoordinateY == dto.TargetY))
            return ServiceResponse<AttackResultDto>.Failure("Ya has atacado esta coordenada.");

        bool isHit = false;
        bool isSunk = false;
        string? shipSunkName = null;

        // Comprobar impacto (Lógica básica)
        Domain.Entities.Battleship.BattleshipShip? hitShip = null;
        foreach (var ship in opponentBoard.Ships)
        {
            var length = (int)ship.Size;
            if (ship.Direction == ShipDirection.Horizontal)
            {
                if (dto.TargetY == ship.StartCoordinateY && dto.TargetX >= ship.StartCoordinateX && dto.TargetX < ship.StartCoordinateX + length)
                {
                    isHit = true;
                    hitShip = ship;
                    break;
                }
            }
            else
            {
                if (dto.TargetX == ship.StartCoordinateX && dto.TargetY >= ship.StartCoordinateY && dto.TargetY < ship.StartCoordinateY + length)
                {
                    isHit = true;
                    hitShip = ship;
                    break;
                }
            }
        }

        var attack = new Domain.Entities.Battleship.BattleshipAttack
        {
            BoardId = opponentBoard.Id,
            CoordinateX = dto.TargetX,
            CoordinateY = dto.TargetY,
            IsHit = isHit
        };
        await _attackRepository.AddAsync(attack);
        opponentBoard.ReceivedAttacks.Add(attack); // Agregar a la colección local para comprobar hundimiento

        if (isHit && hitShip != null)
        {
            // Comprobar si se hundió (Si todos los puntos del barco fueron atacados con éxito)
            isSunk = CheckIfSunk(hitShip, opponentBoard.ReceivedAttacks.ToList());
            if (isSunk)
            {
                hitShip.IsSunk = true;
                await _shipRepository.UpdateAsync(hitShip);
                shipSunkName = hitShip.Size.ToString();
            }
        }

        // Comprobar fin del juego
        bool isGameOver = opponentBoard.Ships.All(s => s.IsSunk);
        
        if (isGameOver)
        {
            game.Status = GameStatus.Finished;
            game.Result = game.Player1Id == dto.AttackerPlayerId ? GameResult.Player1Won : GameResult.Player2Won;
            game.WinnerId = dto.AttackerPlayerId;
        }
        else
        {
            // Cambiar turno
            game.CurrentTurnPlayerId = opponentId;
            
            // Notificar turno al oponente
            await _notificationDispatch.SendNotificationAsync(
                opponentId,
                NotificationType.BattleshipTurn,
                "Es tu turno en Battleship.",
                game.Id.ToString());
        }

        await _gameRepository.UpdateAsync(game);

        return ServiceResponse<AttackResultDto>.Success(new AttackResultDto
        {
            CoordinateX = dto.TargetX,
            CoordinateY = dto.TargetY,
            IsHit = isHit,
            IsSunk = isSunk,
            IsGameOver = isGameOver,
            WinnerId = game.WinnerId,
            ShipSunkName = shipSunkName
        });
    }

    private bool CheckIfSunk(Domain.Entities.Battleship.BattleshipShip ship, List<Domain.Entities.Battleship.BattleshipAttack> allAttacks)
    {
        var length = (int)ship.Size;
        if (ship.Direction == ShipDirection.Horizontal)
        {
            for (int i = 0; i < length; i++)
            {
                if (!allAttacks.Any(a => a.CoordinateX == ship.StartCoordinateX + i && a.CoordinateY == ship.StartCoordinateY))
                    return false;
            }
        }
        else
        {
            for (int i = 0; i < length; i++)
            {
                if (!allAttacks.Any(a => a.CoordinateY == ship.StartCoordinateY + i && a.CoordinateX == ship.StartCoordinateX))
                    return false;
            }
        }
        return true;
    }
}
