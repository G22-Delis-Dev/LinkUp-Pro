using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Reaction;

namespace LinkUpPro.Domain.Entities.Reaction;
public class Reaction : BaseEntity<Guid>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }

    public Post.Post Post { get; set; } = null!;
    public User.User User { get; set; } = null!;
}