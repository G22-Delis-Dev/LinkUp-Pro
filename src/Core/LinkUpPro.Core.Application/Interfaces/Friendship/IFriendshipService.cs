using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Friendship;

namespace LinkUpPro.Application.Interfaces.Friendship;

public interface IFriendshipService
{
    Task<List<FriendshipDto>> GetFriendsAsync(Guid userId);
    Task<BaseResult> RemoveFriendAsync(Guid userId, Guid friendId);
    Task<bool> AreFriendsAsync(Guid userId, Guid friendId);
}
