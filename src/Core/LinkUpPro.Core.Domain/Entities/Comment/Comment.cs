using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Comment;
public class Comment : AuditableEntity<Guid>
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;

    public Post.Post Post { get; set; } = null!;
    public User.User User { get; set; } = null!;
    public ICollection<CommentReply> Replies { get; private set; } = new List<CommentReply>();
}