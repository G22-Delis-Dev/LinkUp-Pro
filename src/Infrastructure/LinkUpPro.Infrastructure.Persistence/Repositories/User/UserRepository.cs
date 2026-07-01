using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User, Guid>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByAppUserIdAsync(string appUserId, CancellationToken cancellationToken = default)
            => await _dbSet
                   .FirstOrDefaultAsync(u => u.AppUserId == appUserId, cancellationToken);

        public async Task<bool> IsActiveAsync(Guid userId)
            => await _dbSet
                   .AnyAsync(u => u.Id == userId && u.IsActive);

        public async Task<IReadOnlyList<User>> GetActiveUsersExceptAsync(Guid excludeUserId)
            => await _dbSet
                   .Where(u => u.IsActive && u.Id != excludeUserId)
                   .OrderBy(u => u.FirstName)
                   .ThenBy(u => u.LastName)
                   .ToListAsync();

        public async Task<User?> GetWithActiveStatusAsync(Guid userId)
            => await _dbSet
                   .Where(u => u.Id == userId && u.IsActive)
                   .FirstOrDefaultAsync();

        public async Task<IReadOnlyList<User>> SearchActiveUsersAsync(
            string query, Guid excludeUserId, CancellationToken cancellationToken = default)
        {
            var q = query.ToLower().Trim();
            return await _dbSet
                .Where(u => u.IsActive
                    && u.Id != excludeUserId
                    && (u.FirstName.ToLower().Contains(q)
                        || u.LastName.ToLower().Contains(q)
                        || (u.FirstName + " " + u.LastName).ToLower().Contains(q)))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Take(20)
                .ToListAsync(cancellationToken);
        }
    }
}
