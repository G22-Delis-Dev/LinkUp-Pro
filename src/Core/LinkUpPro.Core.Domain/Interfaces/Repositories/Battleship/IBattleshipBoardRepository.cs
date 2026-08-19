using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Battleship;

public interface IBattleshipBoardRepository : IGenericRepository<BattleshipBoard, Guid>
{
    Task<BattleshipBoard?> GetByGameAndOwnerAsync(Guid gameId, Guid ownerId);
    Task<bool> BothPlayersReadyAsync(Guid gameId);
}