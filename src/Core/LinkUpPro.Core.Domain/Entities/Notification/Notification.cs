using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Notification;

namespace LinkUpPro.Domain.Entities.Notification;
public class Notification : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? RelatedEntityId { get; set; }

    public User.User User { get; set; } = null!;
}