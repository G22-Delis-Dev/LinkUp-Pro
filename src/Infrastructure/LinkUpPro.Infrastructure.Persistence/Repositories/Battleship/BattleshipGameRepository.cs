using LinkUpPro.Domain.Entities.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
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
    public class BattleshipGameRepository
        : GenericRepository<BattleshipGame, Guid>, IBattleshipGameRepository
    {
        public BattleshipGameRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<BattleshipGame>> GetActiveByPlayerAsync(Guid userId)
            => await _dbSet
                   .Where(g =>
                       (g.Player1Id == userId || g.Player2Id == userId) &&
                       g.Status != GameStatus.Finished)
                   .Include(g => g.Boards)
                   .Include(g => g.Player1)
                   .Include(g => g.Player2)
                   .OrderByDescending(g => g.CreatedAt)
                   .ToListAsync();

        public async Task<IReadOnlyList<BattleshipGame>> GetHistoryByPlayerAsync(Guid userId)
            => await _dbSet
                   .Where(g =>
                       (g.Player1Id == userId || g.Player2Id == userId) &&
                       g.Status == GameStatus.Finished)
                   .OrderByDescending(g => g.LastModifiedAt)
                   .ToListAsync();

        public async Task<BattleshipGame?> GetActiveBetweenAsync(Guid userA, Guid userB)
            => await _dbSet
                   .FirstOrDefaultAsync(g =>
                       ((g.Player1Id == userA && g.Player2Id == userB) ||
                        (g.Player1Id == userB && g.Player2Id == userA)) &&
                       g.Status != GameStatus.Finished);

        public async Task<BattleshipGame?> GetWithBoardsAsync(Guid gameId)
            => await _dbSet
                   .Include(g => g.Boards)
                       .ThenInclude(b => b.Ships)
                   .FirstOrDefaultAsync(g => g.Id == gameId);

        public async Task<bool> HasActiveGameWithAsync(Guid userId, Guid opponentId)
            => await _dbSet.AnyAsync(g =>
                   ((g.Player1Id == userId && g.Player2Id == opponentId) ||
                    (g.Player1Id == opponentId && g.Player2Id == userId)) &&
                   g.Status != GameStatus.Finished);
    }
}
