using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Friendship;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly LinkUpPro.Infrastructure.Shared.Services.Storage.IImageStorageService _imageStorage;

    public FriendshipService(
        IFriendshipRepository friendshipRepository,
        UserManager<AppUser> userManager,
        LinkUpPro.Infrastructure.Shared.Services.Storage.IImageStorageService imageStorage)
    {
        _friendshipRepository = friendshipRepository;
        _userManager = userManager;
        _imageStorage = imageStorage;
    }

    public async Task<List<FriendshipDto>> GetFriendsAsync(Guid userId)
    {
        var friendships = await _friendshipRepository.Query()
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Active)
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();

        var result = new List<FriendshipDto>();

        foreach (var f in friendships)
        {
            var isUser1 = f.UserId == userId;
            var friendUser = isUser1 ? f.Friend : f.User;

            // Obtener el username desde Identity usando AppUserId
            var appUser = await _userManager.FindByIdAsync(friendUser.AppUserId);

            result.Add(new FriendshipDto
            {
                Id = f.Id,
                UserId = userId,
                FriendId = friendUser.Id,
                FriendName = $"{friendUser.FirstName} {friendUser.LastName}",
                FriendUsername = appUser?.UserName,
                FriendProfilePicture = friendUser.ProfilePicturePath != null ? _imageStorage.GetImageUrl(friendUser.ProfilePicturePath) : null,
                Status = f.Status,
                Since = f.CreatedAt
            });
        }

        return result;
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

    public async Task<List<FriendshipDto>> GetFriendsAsync(Guid userId, string? search)
    {
        var friends = await GetFriendsAsync(userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var q = search.Trim().ToLower();
            friends = friends
                .Where(f =>
                    f.FriendName.ToLower().Contains(q) ||
                    (f.FriendUsername != null && f.FriendUsername.ToLower().Contains(q)))
                .ToList();
        }

        return friends.OrderBy(f => f.FriendName).ToList();
    }
}
