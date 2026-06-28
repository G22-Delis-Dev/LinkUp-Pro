using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Comment;

namespace LinkUpPro.Application.Interfaces.Comment;

public interface ICommentReplyService
{
    Task<ServiceResponse<CommentReplyDto>> CreateReplyAsync(CreateCommentReplyDto dto);
    Task<BaseResult> DeleteReplyAsync(Guid replyId, Guid userId);
    Task<List<CommentReplyDto>> GetRepliesByCommentAsync(Guid commentId);
}
