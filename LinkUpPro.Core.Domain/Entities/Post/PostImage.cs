using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Post;
public class PostImage : BaseEntity<Guid>
{
    public Guid PostId { get; set; }
    public string ImagePath { get; set; } = null!;
    public Post Post { get; set; } = null!;
}