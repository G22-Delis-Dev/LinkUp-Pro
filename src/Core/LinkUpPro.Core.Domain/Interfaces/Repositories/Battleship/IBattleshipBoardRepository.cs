using System;
using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Domain.Interfaces.Repositories.Battleship;

public interface IBattleshipBoardRepository : IGenericRepository<BattleshipBoard, Guid>
{
}