using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Battleship;

public interface IBattleshipShipRepository : IGenericRepository<BattleshipShip, Guid>
{
    Task<IReadOnlyList<BattleshipShip>> GetByBoardAsync(Guid boardId);
    Task<IReadOnlyList<BattleshipShip>> GetPlacedByBoardAsync(Guid boardId);
    Task<IReadOnlyList<(int Row, int Col)>> GetOccupiedCellsAsync(Guid boardId);
    Task<bool> AllPlacedAsync(Guid boardId);
}