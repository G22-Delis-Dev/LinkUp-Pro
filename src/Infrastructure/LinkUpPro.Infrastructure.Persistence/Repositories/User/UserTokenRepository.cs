using LinkUpPro.Domain.Entities.User;
using LinkUpPro.Domain.Enums.User;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class UserTokenRepository : GenericRepository<UserToken, Guid>, IUserTokenRepository
    {
        public UserTokenRepository(ApplicationDbContext context) : base(context) { }

        public async Task<UserToken?> GetValidTokenAsync(
            string token, TokenType type)
            => await _dbSet
                   .FirstOrDefaultAsync(t =>
                       t.Token == token &&
                       t.Type == type &&
                       !t.IsUsed &&
                       t.ExpirationDate > DateTime.UtcNow);

        public async Task InvalidateAllAsync(Guid userId, TokenType type)
        {
            var tokens = await _dbSet
                .Where(t => t.UserId == userId && t.Type == type && !t.IsUsed)
                .ToListAsync();

            foreach (var t in tokens)
                t.IsUsed = true;

            await _context.SaveChangesAsync();
        }

        public async Task<DateTime?> GetLastSentAtAsync(Guid userId, TokenType type)
            => await _dbSet
                   .Where(t => t.UserId == userId && t.Type == type)
                   .OrderByDescending(t => t.CreatedAt)
                   .Select(t => (DateTime?)t.CreatedAt)
                   .FirstOrDefaultAsync();
    }
}
