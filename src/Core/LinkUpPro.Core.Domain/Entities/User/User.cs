using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.User;
public class User : AuditableEntity<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? ProfilePicturePath { get; set; }
    public bool IsActive { get; set; } = true;
    public string AppUserId { get; set; } = null!; // Link a Identity

    public ICollection<Post.Post> Posts { get; private set; } = new List<Post.Post>();
    public ICollection<Friendship.Friendship> Friendships { get; private set; } = new List<Friendship.Friendship>();
}