using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Friendship;

namespace LinkUpPro.Domain.Entities.Friendship;
public class Friendship : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Active;

    public User.User User { get; set; } = null!;
    public User.User Friend { get; set; } = null!;
}