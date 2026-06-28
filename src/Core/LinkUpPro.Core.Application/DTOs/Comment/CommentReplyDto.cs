namespace LinkUpPro.Application.DTOs.Comment;

public class CommentReplyDto
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string? AuthorProfilePicture { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
