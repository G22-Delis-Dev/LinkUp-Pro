using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Comment;

namespace LinkUpPro.Application.Interfaces.Comment;

public interface ICommentService
{
    Task<ServiceResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto);
    Task<BaseResult> DeleteCommentAsync(Guid commentId, Guid userId);
    Task<List<CommentDto>> GetCommentsByPostAsync(Guid postId);
}
