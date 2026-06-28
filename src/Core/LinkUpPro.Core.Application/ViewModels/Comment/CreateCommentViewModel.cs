namespace LinkUpPro.Application.ViewModels.Comment;

public class CreateCommentViewModel
{
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;
}
