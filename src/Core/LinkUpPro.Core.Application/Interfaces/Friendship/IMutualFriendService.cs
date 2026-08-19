using LinkUpPro.Application.DTOs.Friendship;

namespace LinkUpPro.Application.Interfaces.Friendship;

public interface IMutualFriendService
{
    Task<List<FriendshipDto>> GetMutualFriendsAsync(Guid userId, Guid otherUserId);
}
