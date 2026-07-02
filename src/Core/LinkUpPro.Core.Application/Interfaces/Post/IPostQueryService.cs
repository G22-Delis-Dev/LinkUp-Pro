using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Post;

namespace LinkUpPro.Application.Interfaces.Post;

public interface IPostQueryService
{
    Task<ServiceResponse<IReadOnlyList<PostDto>>> GetFeedAsync(Guid userId);
    Task<ServiceResponse<IReadOnlyList<PostDto>>> GetUserPostsAsync(Guid targetUserId, Guid requestingUserId);
    Task<ServiceResponse<PostDto>> GetPostByIdAsync(Guid postId, Guid requestingUserId);
}
