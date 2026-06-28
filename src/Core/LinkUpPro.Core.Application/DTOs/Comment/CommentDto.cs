namespace LinkUpPro.Application.DTOs.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string? AuthorProfilePicture { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int ReplyCount { get; set; }
}
