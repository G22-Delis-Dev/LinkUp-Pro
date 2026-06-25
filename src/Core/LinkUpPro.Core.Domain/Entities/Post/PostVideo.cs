using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Post;
public class PostVideo : BaseEntity<Guid>
{
    public Guid PostId { get; set; }
    public string VideoPath { get; set; } = null!;
    public Post Post { get; set; } = null!;
}