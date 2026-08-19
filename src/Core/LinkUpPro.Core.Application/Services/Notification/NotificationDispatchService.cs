using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Notification;

namespace LinkUpPro.Application.Services.Notification;

public class NotificationDispatchService : INotificationDispatchService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationDispatchService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task SendNotificationAsync(Guid userId, NotificationType type, string message, string? relatedEntityId = null)
    {
        var notification = new Domain.Entities.Notification.Notification
        {
            UserId = userId,
            Type = type,
            Message = message,
            RelatedEntityId = relatedEntityId,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);
    }
}
