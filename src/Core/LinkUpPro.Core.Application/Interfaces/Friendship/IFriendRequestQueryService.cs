using LinkUpPro.Application.DTOs.Friendship;

namespace LinkUpPro.Application.Interfaces.Friendship;

public interface IFriendRequestQueryService
{
    Task<List<FriendRequestDto>> GetReceivedRequestsAsync(Guid userId);
    Task<List<FriendRequestDto>> GetSentRequestsAsync(Guid userId);
    Task<bool> HasPendingRequestAsync(Guid senderId, Guid receiverId);
}
