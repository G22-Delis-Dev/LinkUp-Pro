using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Battleship;

public interface IBattleshipGameRepository : IGenericRepository<BattleshipGame, Guid>
{
    Task<IReadOnlyList<BattleshipGame>> GetActiveByPlayerAsync(Guid userId);
    Task<IReadOnlyList<BattleshipGame>> GetHistoryByPlayerAsync(Guid userId);
    Task<BattleshipGame?> GetActiveBetweenAsync(Guid userA, Guid userB);
    Task<BattleshipGame?> GetWithBoardsAsync(Guid gameId);
    Task<bool> HasActiveGameWithAsync(Guid userId, Guid opponentId);
}