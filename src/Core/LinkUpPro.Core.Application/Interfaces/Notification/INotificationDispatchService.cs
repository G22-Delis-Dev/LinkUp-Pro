using LinkUpPro.Domain.Enums.Notification;

namespace LinkUpPro.Application.Interfaces.Notification;

public interface INotificationDispatchService
{
    // Servicio interno para enviar notificaciones desde otras capas/servicios
    Task SendNotificationAsync(Guid userId, NotificationType type, string message, string? relatedEntityId = null);
}
