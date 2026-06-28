using LinkUpPro.Domain.Enums.Post;

namespace LinkUpPro.Application.DTOs.Post;

public class UpdatePostDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; }
    public bool AllowComments { get; set; }
}
