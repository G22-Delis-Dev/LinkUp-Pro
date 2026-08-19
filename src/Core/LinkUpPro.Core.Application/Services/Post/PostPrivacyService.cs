using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Domain.Enums.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Post;

namespace LinkUpPro.Application.Services.Post;

public class PostPrivacyService : IPostPrivacyService
{
    private readonly IPostRepository _postRepository;
    private readonly IFriendshipRepository _friendshipRepository;

    public PostPrivacyService(
        IPostRepository postRepository,
        IFriendshipRepository friendshipRepository)
    {
        _postRepository = postRepository;
        _friendshipRepository = friendshipRepository;
    }

    public async Task<bool> CanViewPostAsync(Guid postId, Guid requestingUserId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null) return false;

        // El autor siempre puede ver sus propios posts
        if (post.UserId == requestingUserId) return true;

        // Posts privados: solo el autor
        if (post.Privacy == PostPrivacy.Private) return false;

        // Posts de solo amigos: verificar amistad
        if (post.Privacy == PostPrivacy.FriendsOnly)
        {
            var areFriends = await _friendshipRepository.ExistsAsync(f =>
                ((f.UserId == requestingUserId && f.FriendId == post.UserId) ||
                 (f.UserId == post.UserId && f.FriendId == requestingUserId)) &&
                f.Status == Domain.Enums.Friendship.FriendshipStatus.Active);

            return areFriends;
        }

        // Posts públicos
        return true;
    }
}
