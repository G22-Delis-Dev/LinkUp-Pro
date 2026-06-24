using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Friendship;

namespace LinkUpPro.Domain.Entities.Friendship;
public class FriendRequest : AuditableEntity<Guid>
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

    public User.User Sender { get; set; } = null!;
    public User.User Receiver { get; set; } = null!;
}