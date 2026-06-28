using LinkUpPro.Domain.Enums.Notification;

namespace LinkUpPro.Application.ViewModels.Notification;

public class NotificationViewModel
{
    public Guid Id { get; set; }
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? RelatedEntityId { get; set; }
    public string TimeAgo { get; set; } = null!;
}
