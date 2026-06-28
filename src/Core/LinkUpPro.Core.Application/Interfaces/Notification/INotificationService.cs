using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Notification;

namespace LinkUpPro.Application.Interfaces.Notification;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsAsync(Guid userId);
    Task<BaseResult> MarkAsReadAsync(Guid notificationId, Guid userId);
    Task<BaseResult> MarkAllAsReadAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
}
