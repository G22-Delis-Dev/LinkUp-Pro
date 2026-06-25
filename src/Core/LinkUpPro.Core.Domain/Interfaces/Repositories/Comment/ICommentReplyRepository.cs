using System;
using LinkUpPro.Domain.Entities.Comment;

namespace LinkUpPro.Domain.Interfaces.Repositories.Comment;

public interface ICommentReplyRepository : IGenericRepository<CommentReply, Guid>
{
}