using LinkUpPro.Domain.Enums.Friendship;

namespace LinkUpPro.Application.DTOs.Friendship;

public class FriendshipDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
    public string FriendName { get; set; } = null!;
    public string? FriendProfilePicture { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime Since { get; set; }
}
