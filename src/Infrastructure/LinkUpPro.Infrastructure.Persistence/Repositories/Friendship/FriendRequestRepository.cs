using LinkUpPro.Domain.Entities.Friendship;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class FriendRequestRepository : GenericRepository<FriendRequest, Guid>, IFriendRequestRepository
    {
        public FriendRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<FriendRequest>> GetPendingReceivedAsync(Guid receiverId)
            => await _dbSet
                   .Where(r =>
                       r.ReceiverId == receiverId &&
                       r.Status == FriendRequestStatus.Pending)
                   .OrderByDescending(r => r.CreatedAt)
                   .ToListAsync();

        public async Task<IReadOnlyList<FriendRequest>> GetVisibleSentAsync(Guid senderId)
            => await _dbSet
                   .Where(r =>
                       r.SenderId == senderId &&
                       r.Status != FriendRequestStatus.Canceled)
                   .OrderByDescending(r => r.CreatedAt)
                   .ToListAsync();

        public async Task<FriendRequest?> GetPendingBetweenAsync(Guid userA, Guid userB)
            => await _dbSet
                   .FirstOrDefaultAsync(r =>
                       r.Status == FriendRequestStatus.Pending &&
                       ((r.SenderId == userA && r.ReceiverId == userB) ||
                        (r.SenderId == userB && r.ReceiverId == userA)));

        public async Task<int> CountPendingReceivedAsync(Guid receiverId)
            => await _dbSet
                   .CountAsync(r =>
                       r.ReceiverId == receiverId &&
                       r.Status == FriendRequestStatus.Pending);
    }
}
