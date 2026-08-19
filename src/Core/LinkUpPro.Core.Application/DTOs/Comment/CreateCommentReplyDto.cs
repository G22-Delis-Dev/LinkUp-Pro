namespace LinkUpPro.Application.DTOs.Comment;

public class CreateCommentReplyDto
{
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
}
