using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Friendship;

public class MutualFriendService : IMutualFriendService
{
    private readonly IFriendshipRepository _friendshipRepository;

    public MutualFriendService(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<List<FriendshipDto>> GetMutualFriendsAsync(Guid userId, Guid otherUserId)
    {
        var userFriends = await _friendshipRepository.Query()
            .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == FriendshipStatus.Active)
            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
            .ToListAsync();

        var otherUserFriends = await _friendshipRepository.Query()
            .Where(f => (f.UserId == otherUserId || f.FriendId == otherUserId) && f.Status == FriendshipStatus.Active)
            .Select(f => f.UserId == otherUserId ? f.FriendId : f.UserId)
            .ToListAsync();

        var mutualFriendIds = userFriends.Intersect(otherUserFriends).ToList();

        if (!mutualFriendIds.Any())
            return new List<FriendshipDto>();

        // Buscar información de esos usuarios
        // En una app real, podrías inyectar IUserRepository o consultarlo de forma más eficiente.
        // Aquí lo haremos consultando las amistades para reusar el DTO y relaciones.
        var mutualFriendships = await _friendshipRepository.Query()
            .Where(f => f.UserId == userId && mutualFriendIds.Contains(f.FriendId) ||
                        f.FriendId == userId && mutualFriendIds.Contains(f.UserId))
            .Include(f => f.User)
            .Include(f => f.Friend)
            .ToListAsync();

        return mutualFriendships.Select(f =>
        {
            var friendUser = f.UserId == userId ? f.Friend : f.User;
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
}
