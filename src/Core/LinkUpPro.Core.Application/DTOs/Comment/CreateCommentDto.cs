namespace LinkUpPro.Application.DTOs.Comment;

public class CreateCommentDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
}
