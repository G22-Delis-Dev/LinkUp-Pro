using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Post;
using System.Xml.Linq;

namespace LinkUpPro.Domain.Entities.Post;
public class Post : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; } = PostPrivacy.FriendsOnly;
    public PostContentType ContentType { get; set; } = PostContentType.Text;
    public bool AllowComments { get; set; } = true;

    public User.User User { get; set; } = null!;
    public ICollection<PostImage> Images { get; private set; } = new List<PostImage>();
    public ICollection<PostVideo> Videos { get; private set; } = new List<PostVideo>();
    public ICollection<Comment.Comment> Comments { get; private set; } = new List<Comment.Comment>();
    public ICollection<Reaction.Reaction> Reactions { get; private set; } = new List<Reaction.Reaction>();
}