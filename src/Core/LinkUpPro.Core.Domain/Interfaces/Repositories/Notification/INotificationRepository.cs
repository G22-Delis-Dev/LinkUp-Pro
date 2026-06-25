using System;
using LinkUpPro.Domain.Entities.Notification;

namespace LinkUpPro.Domain.Interfaces.Repositories.Notification;

public interface INotificationRepository : IGenericRepository<Entities.Notification.Notification, Guid>
{
}