using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Battleship;

public class BattleshipHistoryService : IBattleshipHistoryService
{
    private readonly IBattleshipGameRepository _gameRepository;

    public BattleshipHistoryService(IBattleshipGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<List<BattleshipGameDto>> GetGameHistoryAsync(Guid playerId)
    {
        var games = await _gameRepository.Query()
            .Where(g => g.Player1Id == playerId || g.Player2Id == playerId)
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
            WinnerId = g.WinnerId,
            CreatedAt = g.CreatedAt
        }).ToList();
    }

    public async Task<BattleshipGameDto?> GetGameDetailsAsync(Guid gameId)
    {
        var game = await _gameRepository.Query()
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null) return null;

        return new BattleshipGameDto
        {
            Id = game.Id,
            Player1Id = game.Player1Id,
            Player1Name = $"{game.Player1.FirstName} {game.Player1.LastName}",
            Player2Id = game.Player2Id,
            Player2Name = $"{game.Player2.FirstName} {game.Player2.LastName}",
            Status = game.Status,
            Result = game.Result,
            WinnerId = game.WinnerId,
            CreatedAt = game.CreatedAt
        };
    }
}
