using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Comment;
public class CommentReply : AuditableEntity<Guid>
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;

    public Comment Comment { get; set; } = null!;
    public User.User User { get; set; } = null!;
}