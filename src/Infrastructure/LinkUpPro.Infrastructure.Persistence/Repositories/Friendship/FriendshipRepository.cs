using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.Friendship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class FriendshipRepository : GenericRepository<Friendship, Guid>, IFriendshipRepository
    {
        public FriendshipRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Friendship?> GetBetweenAsync(Guid userA, Guid userB)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f =>
                    (f.UserId == userA && f.FriendId == userB) ||
                    (f.UserId == userB && f.FriendId == userA));
        }

        public async Task<bool> AreActiveFriendsAsync(Guid userA, Guid userB)
        {
            return await _dbSet.AnyAsync(f =>
                ((f.UserId == userA && f.FriendId == userB) ||
                 (f.UserId == userB && f.FriendId == userA)) &&
                f.Status == FriendshipStatus.Active);
        }

        public async Task<IReadOnlyList<Guid>> GetFriendIdsAsync(Guid userId)
            => await _dbSet
                   .Where(f =>
                       (f.UserId == userId || f.FriendId == userId) &&
                       f.Status == FriendshipStatus.Active)
                   .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
                   .ToListAsync();

        public async Task<IReadOnlyList<Guid>> GetMutualFriendIdsAsync(
            Guid userA, Guid userB)
        {
            var friendsA = await GetFriendIdsAsync(userA);
            var friendsB = await GetFriendIdsAsync(userB);
            return friendsA
                .Intersect(friendsB)
                .Where(id => id != userA && id != userB)
                .ToList();
        }

        public async Task<IReadOnlyList<Friendship>> GetAllActiveByUserAsync(Guid userId)
            => await _dbSet
                   .Where(f =>
                       (f.UserId == userId || f.FriendId == userId) &&
                       f.Status == FriendshipStatus.Active)
                   .OrderBy(f => f.CreatedAt)
                   .ToListAsync();
    }
}
