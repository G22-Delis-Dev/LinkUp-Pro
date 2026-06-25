using LinkUpPro.Domain.Interfaces.Repositories.Notification;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification, Guid>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Notification>> GetByRecipientAsync(Guid recipientId)
            => await _dbSet
                   .Where(n => n.UserId == recipientId)
                   .OrderByDescending(n => n.CreatedAt)
                   .ToListAsync();

        public async Task<int> GetUnreadCountAsync(Guid recipientId)
            => await _dbSet
                   .CountAsync(n =>
                       n.UserId == recipientId &&
                       !n.IsRead);

        public async Task MarkAllAsReadAsync(Guid recipientId)
        {
            var unread = await _dbSet
                .Where(n => n.UserId == recipientId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }
    }
}
