using LinkUpPro.Domain.Enums.Friendship;

namespace LinkUpPro.Application.DTOs.Friendship;

public class FriendRequestDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; } = null!;
    public string? SenderProfilePicture { get; set; }
    public Guid ReceiverId { get; set; }
    public FriendRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
