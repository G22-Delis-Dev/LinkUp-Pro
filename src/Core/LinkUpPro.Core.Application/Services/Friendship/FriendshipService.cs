using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Friendship;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;

    public FriendshipService(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<List<FriendshipDto>> GetFriendsAsync(Guid userId)
    {
        var friendships = await _friendshipRepository.Query()
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Active)
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();

        return friendships.Select(f =>
        {
            var isUser1 = f.UserId == userId;
            var friendUser = isUser1 ? f.Friend : f.User;

            return new FriendshipDto
            {
                Id = f.Id,
                UserId = userId,
                FriendId = friendUser.Id,
                FriendName = $"{friendUser.FirstName} {friendUser.LastName}",
                FriendProfilePicture = friendUser.ProfilePicturePath,
                Status = f.Status,
                Since = f.CreatedAt
            };
        }).ToList();
    }

    public async Task<BaseResult> RemoveFriendAsync(Guid userId, Guid friendId)
    {
        var friendship = await _friendshipRepository.FindOneAsync(f =>
            (f.UserId == userId && f.FriendId == friendId) ||
            (f.UserId == friendId && f.FriendId == userId));

        if (friendship == null)
            return BaseResult.Fail("No son amigos.");

        await _friendshipRepository.DeleteAsync(friendship);

        return BaseResult.Ok();
    }

    public async Task<bool> AreFriendsAsync(Guid userId, Guid friendId)
    {
        return await _friendshipRepository.ExistsAsync(f =>
            ((f.UserId == userId && f.FriendId == friendId) ||
             (f.UserId == friendId && f.FriendId == userId)) &&
            f.Status == FriendshipStatus.Active);
    }
}
