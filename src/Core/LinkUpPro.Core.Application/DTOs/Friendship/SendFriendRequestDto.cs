namespace LinkUpPro.Application.DTOs.Friendship;

public class SendFriendRequestDto
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
}
