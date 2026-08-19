using LinkUpPro.Domain.Enums.Post;

namespace LinkUpPro.Application.DTOs.Post;

public class CreatePostDto
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; } = PostPrivacy.FriendsOnly;
    public PostContentType ContentType { get; set; }
    public bool AllowComments { get; set; } = true;

    // Imagen (Texto + Imagen)
    public Stream? ImageStream { get; set; }
    public string? ImageContentType { get; set; }
    public string? ImageFileName { get; set; }

    // Video de YouTube (Texto + Video)
    public string? YouTubeUrl { get; set; }
}
