using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Post;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Domain.Enums.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Shared.Helpers;
using LinkUpPro.Infrastructure.Shared.Services.Storage;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Post;

public class PostQueryService : IPostQueryService
{
    private readonly IPostRepository _postRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImageStorageService _imageStorage;
    private readonly IPostPrivacyService _privacyService;

    public PostQueryService(
        IPostRepository postRepository,
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        IImageStorageService imageStorage,
        IPostPrivacyService privacyService)
    {
        _postRepository = postRepository;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _imageStorage = imageStorage;
        _privacyService = privacyService;
    }

    public async Task<ServiceResponse<IReadOnlyList<PostDto>>> GetFeedAsync(Guid userId)
    {
        // 1. Obtener IDs de amigos activos
        var friendships = await _friendshipRepository.FindAsync(f =>
            (f.UserId == userId || f.FriendId == userId) &&
            f.Status == Domain.Enums.Friendship.FriendshipStatus.Active);

        var friendIds = friendships
            .Select(f => f.UserId == userId ? f.FriendId : f.UserId)
            .ToHashSet();

        // 2. Obtener posts: propios + de amigos (FriendsOnly)
        var posts = await _postRepository.Query()
            .Include(p => p.Images)
            .Include(p => p.Videos)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Where(p =>
                // Posts propios (cualquier privacidad)
                p.UserId == userId ||
                // Posts de amigos (solo FriendsOnly)
                (friendIds.Contains(p.UserId) && p.Privacy == PostPrivacy.FriendsOnly))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // 3. Obtener datos de los autores
        var authorIds = posts.Select(p => p.UserId).Distinct().ToList();
        var authors = new Dictionary<Guid, Domain.Entities.User.User>();
        foreach (var authorId in authorIds)
        {
            var author = await _userRepository.GetByIdAsync(authorId);
            if (author != null) authors[authorId] = author;
        }

        // 4. Mapear a DTOs
        var postDtos = posts.Select(p =>
        {
            var author = authors.GetValueOrDefault(p.UserId);
            var image = p.Images.FirstOrDefault();
            var video = p.Videos.FirstOrDefault();

            return new PostDto
            {
                Id = p.Id,
                UserId = p.UserId,
                AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : "Usuario",
                AuthorProfilePicture = author?.ProfilePicturePath != null
                    ? _imageStorage.GetImageUrl(author.ProfilePicturePath)
                    : null,
                Content = p.Content,
                Privacy = p.Privacy,
                ContentType = p.ContentType,
                AllowComments = p.AllowComments,
                ImageUrl = image != null ? _imageStorage.GetImageUrl(image.ImagePath) : null,
                YouTubeVideoId = video?.VideoPath,
                CommentCount = p.Comments.Count,
                LikeCount = p.Reactions.Count(r => r.Type == Domain.Enums.Reaction.ReactionType.Like),
                DislikeCount = p.Reactions.Count(r => r.Type == Domain.Enums.Reaction.ReactionType.Dislike),
                CreatedAt = p.CreatedAt,
                LastModifiedAt = p.LastModifiedAt
            };
        }).ToList();

        return ServiceResponse<IReadOnlyList<PostDto>>.Success(postDtos);
    }

    public async Task<ServiceResponse<PostDto>> GetPostByIdAsync(Guid postId, Guid requestingUserId)
    {
        var post = await _postRepository.Query()
            .Include(p => p.Images)
            .Include(p => p.Videos)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            return ServiceResponse<PostDto>.Failure("Publicación no encontrada.");
        }

        // Verificar permisos de privacidad
        if (!await _privacyService.CanViewPostAsync(postId, requestingUserId))
        {
            return ServiceResponse<PostDto>.Failure(
                "No tienes permisos para ver esta publicación.");
        }

        var author = await _userRepository.GetByIdAsync(post.UserId);
        var image = post.Images.FirstOrDefault();
        var video = post.Videos.FirstOrDefault();

        var dto = new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : "Usuario",
            AuthorProfilePicture = author?.ProfilePicturePath != null
                ? _imageStorage.GetImageUrl(author.ProfilePicturePath)
                : null,
            Content = post.Content,
            Privacy = post.Privacy,
            ContentType = post.ContentType,
            AllowComments = post.AllowComments,
            ImageUrl = image != null ? _imageStorage.GetImageUrl(image.ImagePath) : null,
            YouTubeVideoId = video?.VideoPath,
            CommentCount = post.Comments.Count,
            LikeCount = post.Reactions.Count(r => r.Type == Domain.Enums.Reaction.ReactionType.Like),
            DislikeCount = post.Reactions.Count(r => r.Type == Domain.Enums.Reaction.ReactionType.Dislike),
            CreatedAt = post.CreatedAt,
            LastModifiedAt = post.LastModifiedAt
        };

        return ServiceResponse<PostDto>.Success(dto);
    }
}
