using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Post;

namespace LinkUpPro.Application.Interfaces.Post;

public interface IPostService
{
    Task<ServiceResponse<PostDto>> CreatePostAsync(CreatePostDto dto);
    Task<BaseResult> UpdatePostAsync(UpdatePostDto dto);
    Task<BaseResult> DeletePostAsync(Guid postId, Guid userId);
}
