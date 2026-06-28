using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipGameService : IBattleshipGameService
{
    private readonly IBattleshipGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationDispatchService _notificationDispatch;

    public BattleshipGameService(
        IBattleshipGameRepository gameRepository,
        IUserRepository userRepository,
        INotificationDispatchService notificationDispatch)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _notificationDispatch = notificationDispatch;
    }

    public async Task<ServiceResponse<BattleshipGameDto>> CreateGameAsync(Guid player1Id, Guid opponentId)
    {
        if (player1Id == opponentId)
            return ServiceResponse<BattleshipGameDto>.Failure("No puedes jugar contra ti mismo.");

        // Validar si ya hay un juego activo entre ellos
        var existingGame = await _gameRepository.FindOneAsync(g =>
            ((g.Player1Id == player1Id && g.Player2Id == opponentId) ||
             (g.Player1Id == opponentId && g.Player2Id == player1Id)) &&
            (g.Status == GameStatus.WaitingForOpponent || g.Status == GameStatus.PlacingShips || g.Status == GameStatus.InProgress));

        if (existingGame != null)
            return ServiceResponse<BattleshipGameDto>.Failure("Ya tienen una partida activa.");

        var game = new Domain.Entities.Battleship.BattleshipGame
        {
            Player1Id = player1Id,
            Player2Id = opponentId,
            Status = GameStatus.PlacingShips, // Directamente a colocar barcos para simplificar
            CurrentTurnPlayerId = player1Id // P1 empieza (aunque no aplica hasta InProgress)
        };

        await _gameRepository.AddAsync(game);

        var p1 = await _userRepository.GetByIdAsync(player1Id);
        var p2 = await _userRepository.GetByIdAsync(opponentId);

        await _notificationDispatch.SendNotificationAsync(
            opponentId,
            NotificationType.BattleshipChallenge,
            $"{p1?.FirstName} {p1?.LastName} te ha retado a una partida de Battleship.",
            game.Id.ToString());

        var dto = new BattleshipGameDto
        {
            Id = game.Id,
            Player1Id = game.Player1Id,
            Player1Name = $"{p1?.FirstName} {p1?.LastName}",
            Player2Id = game.Player2Id,
            Player2Name = $"{p2?.FirstName} {p2?.LastName}",
            Status = game.Status,
            Result = game.Result,
            CurrentTurnPlayerId = game.CurrentTurnPlayerId,
            CreatedAt = game.CreatedAt
        };

        return ServiceResponse<BattleshipGameDto>.Success(dto);
    }

    public async Task<ServiceResponse<BattleshipGameDto>> GetGameAsync(Guid gameId)
    {
        var game = await _gameRepository.Query()
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
            return ServiceResponse<BattleshipGameDto>.Failure("Partida no encontrada.");

        var dto = new BattleshipGameDto
        {
            Id = game.Id,
            Player1Id = game.Player1Id,
            Player1Name = $"{game.Player1.FirstName} {game.Player1.LastName}",
            Player2Id = game.Player2Id,
            Player2Name = $"{game.Player2.FirstName} {game.Player2.LastName}",
            Status = game.Status,
            Result = game.Result,
            CurrentTurnPlayerId = game.CurrentTurnPlayerId,
            WinnerId = game.WinnerId,
            CreatedAt = game.CreatedAt
        };

        return ServiceResponse<BattleshipGameDto>.Success(dto);
    }

    public async Task<List<BattleshipGameDto>> GetActiveGamesAsync(Guid playerId)
    {
        var games = await _gameRepository.Query()
            .Where(g => (g.Player1Id == playerId || g.Player2Id == playerId) &&
                        g.Status != GameStatus.Finished && g.Status != GameStatus.Canceled)
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();

        return games.Select(g => new BattleshipGameDto
        {
            Id = g.Id,
            Player1Id = g.Player1Id,
            Player1Name = $"{g.Player1.FirstName} {g.Player1.LastName}",
            Player2Id = g.Player2Id,
            Player2Name = $"{g.Player2.FirstName} {g.Player2.LastName}",
            Status = g.Status,
            Result = g.Result,
            CurrentTurnPlayerId = g.CurrentTurnPlayerId,
            CreatedAt = g.CreatedAt
        }).ToList();
    }

    public async Task<BaseResult> CancelGameAsync(Guid gameId, Guid playerId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null) return BaseResult.Fail("Partida no encontrada.");

        if (game.Player1Id != playerId && game.Player2Id != playerId)
            return BaseResult.Fail("No tienes permisos en esta partida.");

        game.Status = GameStatus.Canceled;
        game.Result = GameResult.Abandoned;
        game.WinnerId = game.Player1Id == playerId ? game.Player2Id : game.Player1Id; // Gana el otro por abandono

        await _gameRepository.UpdateAsync(game);

        return BaseResult.Ok();
    }
}
