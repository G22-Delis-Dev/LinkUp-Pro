namespace LinkUpPro.Application.ViewModels.Comment;

public class CreateCommentReplyViewModel
{
    public Guid CommentId { get; set; }
    public string Content { get; set; } = null!;
}
