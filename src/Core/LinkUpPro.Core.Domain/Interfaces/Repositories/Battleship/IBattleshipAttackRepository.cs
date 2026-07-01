using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Battleship;

public interface IBattleshipAttackRepository : IGenericRepository<BattleshipAttack, Guid>
{
    Task<IReadOnlyList<BattleshipAttack>> GetByGameAndAttackerAsync(Guid gameId, Guid attackerId);
    Task<bool> HasAttackedCellAsync(Guid gameId, Guid attackerId, int row, int col);
    Task<int> CountHitsAsync(Guid gameId, Guid attackerId);
    Task<bool> AllShipsSunkAsync(Guid gameId, Guid attackerId, IReadOnlyList<(int Row, int Col)> opponentCells);
}