using LinkUpPro.Domain.Enums.Post;

namespace LinkUpPro.Application.ViewModels.Post;

public class PostViewModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public string? AuthorProfilePicture { get; set; }
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; }
    public PostContentType ContentType { get; set; }
    public bool AllowComments { get; set; }
    public string? ImageUrl { get; set; }
    public string? YouTubeVideoId { get; set; }
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public bool CurrentUserHasLiked { get; set; }
    public bool CurrentUserHasDisliked { get; set; }
    public string TimeAgo { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsOwner { get; set; }
    public List<LinkUpPro.Application.DTOs.Comment.CommentDto> Comments { get; set; } = new();
}
