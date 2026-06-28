using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Notification;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Notification;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Notification;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId)
    {
        var notifications = await _notificationRepository.Query()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            RelatedEntityId = n.RelatedEntityId,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task<BaseResult> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null) return BaseResult.Fail("Notificación no encontrada.");

        if (notification.UserId != userId) return BaseResult.Fail("No autorizado.");

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification);

        return BaseResult.Ok();
    }

    public async Task<BaseResult> MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }

        return BaseResult.Ok();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _notificationRepository.Query()
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }
}
