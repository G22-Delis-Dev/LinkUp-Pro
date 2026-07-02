using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Exceptions;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Domain.Rules.Battleship.Game;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipGameService : IBattleshipGameService
{
    private readonly IBattleshipGameRepository _gameRepo;
    private readonly IBattleshipBoardRepository _boardRepo;
    private readonly IBattleshipShipRepository _shipRepo;
    private readonly IFriendshipRepository _friendshipRepo;
    private readonly INotificationDispatchService _notificationDispatch;
    private readonly IUserRepository _userRepository;

    private static readonly int[] DefaultShipSizes = { 2, 3, 3, 4, 5 };

    public BattleshipGameService(
        IBattleshipGameRepository gameRepo,
        IBattleshipBoardRepository boardRepo,
        IBattleshipShipRepository shipRepo,
        IFriendshipRepository friendshipRepo,
        INotificationDispatchService notificationDispatch,
        IUserRepository userRepository)
    {
        _gameRepo = gameRepo;
        _boardRepo = boardRepo;
        _shipRepo = shipRepo;
        _friendshipRepo = friendshipRepo;
        _notificationDispatch = notificationDispatch;
        _userRepository = userRepository;
    }

    public async Task<ServiceResponse<BattleshipGameDto>> CreateGameAsync(Guid creatorId, Guid opponentId)
    {
        try
        {
            var areFriends = await _friendshipRepo.AreActiveFriendsAsync(creatorId, opponentId);
            if (!areFriends)
                return ServiceResponse<BattleshipGameDto>.Failure("Solo puedes iniciar una partida con un amigo activo.");

            var hasActive = await _gameRepo.HasActiveGameWithAsync(creatorId, opponentId);
            RuleValidator.CheckRule(new NoActiveGameWithSameOpponentRule(hasActive));

            var game = new BattleshipGame
            {
                Id = Guid.NewGuid(),
                Player1Id = creatorId,
                Player2Id = opponentId,
                Status = GameStatus.PlacingShips,
                CurrentTurnPlayerId = creatorId,
                TurnStartedAt = DateTime.UtcNow
            };
            await _gameRepo.AddAsync(game);

            await CreateBoardAsync(game.Id, creatorId);
            await CreateBoardAsync(game.Id, opponentId);

            // Notificar al oponente que recibió un desafío
            var creator = await _userRepository.GetByIdAsync(creatorId);
            var creatorName = creator != null ? $"{creator.FirstName} {creator.LastName}" : "Un jugador";
            await _notificationDispatch.SendNotificationAsync(
                opponentId,
                NotificationType.BattleshipChallenge,
                $"{creatorName} te ha desafiado a una partida de Battleship.",
                game.Id.ToString());

            var dto = new BattleshipGameDto
            {
                Id = game.Id,
                Player1Id = game.Player1Id,
                Player2Id = game.Player2Id,
                Status = game.Status,
                CurrentTurnPlayerId = game.CurrentTurnPlayerId
            };

            return ServiceResponse<BattleshipGameDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ServiceResponse<BattleshipGameDto>.Failure(ex.Message);
        }
    }

    public async Task<BaseResult> SurrenderAsync(Guid gameId, Guid surrenderingUserId)
    {
        var game = await _gameRepo.GetByIdAsync(gameId);
        if (game == null)
            return BaseResult.Fail("La partida no existe.");

        if (game.Player1Id != surrenderingUserId && game.Player2Id != surrenderingUserId)
            return BaseResult.Fail("No eres participante de esta partida.");

        if (game.Status == GameStatus.Finished || game.Status == GameStatus.Canceled)
            return BaseResult.Fail("La partida ya termino.");

        var winnerId = surrenderingUserId == game.Player1Id
            ? game.Player2Id
            : game.Player1Id;

        game.Status = GameStatus.Finished;
        game.WinnerId = winnerId;
        game.Result = GameResult.None;
        await _gameRepo.UpdateAsync(game);

        return BaseResult.Ok();
    }

    public async Task CheckAndApplyTimeoutAsync(Guid gameId)
    {
        var game = await _gameRepo.GetByIdAsync(gameId);
        if (game == null || game.Status == GameStatus.Finished) return;

        if (game.TurnStartedAt.HasValue &&
            DateTime.UtcNow - game.TurnStartedAt.Value > TimeSpan.FromHours(48))
        {
            var winnerId = game.CurrentTurnPlayerId == game.Player1Id
                ? game.Player2Id
                : game.Player1Id;

            game.Status = GameStatus.Finished;
            game.WinnerId = winnerId;
            await _gameRepo.UpdateAsync(game);
        }
    }

    public async Task<bool> IsParticipantAsync(Guid gameId, Guid userId)
    {
        var game = await _gameRepo.GetByIdAsync(gameId);
        return game != null && (game.Player1Id == userId || game.Player2Id == userId);
    }

    public async Task<ServiceResponse<BattleshipGameDto>> GetGameAsync(Guid gameId)
    {
        var game = await _gameRepo.GetWithBoardsAsync(gameId);
        if (game == null)
            return ServiceResponse<BattleshipGameDto>.Failure("Partida no encontrada.");

        return ServiceResponse<BattleshipGameDto>.Success(MapToDto(game));
    }

    public async Task<List<BattleshipGameDto>> GetActiveGamesAsync(Guid userId)
    {
        var games = await _gameRepo.GetActiveByPlayerAsync(userId);
        return games.Select(MapToDto).ToList();
    }

    private static BattleshipGameDto MapToDto(BattleshipGame g) => new()
    {
        Id = g.Id,
        Player1Id = g.Player1Id,
        Player1Name = g.Player1 != null ? $"{g.Player1.FirstName} {g.Player1.LastName}" : "",
        Player2Id = g.Player2Id,
        Player2Name = g.Player2 != null ? $"{g.Player2.FirstName} {g.Player2.LastName}" : "",
        Status = g.Status,
        Result = g.Result,
        WinnerId = g.WinnerId,
        CurrentTurnPlayerId = g.CurrentTurnPlayerId
    };

    private async Task CreateBoardAsync(Guid gameId, Guid ownerId)
    {
        var board = new BattleshipBoard
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            PlayerId = ownerId
        };
        await _boardRepo.AddAsync(board);

        foreach (var size in DefaultShipSizes)
        {
            await _shipRepo.AddAsync(new BattleshipShip
            {
                Id = Guid.NewGuid(),
                BoardId = board.Id,
                Size = (ShipSize)size,
                StartCoordinateX = -1,  // -1 = no posicionado todavía
                StartCoordinateY = -1
            });
        }
    }
}
