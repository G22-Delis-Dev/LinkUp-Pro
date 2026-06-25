using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Interfaces.Repositories.Battleship;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class BattleshipBoardRepository
        : GenericRepository<BattleshipBoard, Guid>, IBattleshipBoardRepository
    {
        public BattleshipBoardRepository(ApplicationDbContext context) : base(context) { }

        public async Task<BattleshipBoard?> GetByGameAndOwnerAsync(
            Guid gameId, Guid ownerId)
            => await _dbSet
                   .Include(b => b.Ships)
                   .FirstOrDefaultAsync(b =>
                       b.GameId == gameId &&
                       b.PlayerId == ownerId);

        public async Task<bool> BothPlayersReadyAsync(Guid gameId)
            => await _dbSet
                   .CountAsync(b =>
                       b.GameId == gameId &&
                       b.Ships.Count == 5) == 2;
    }
}
