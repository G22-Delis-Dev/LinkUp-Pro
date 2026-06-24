using System;
using LinkUpPro.Domain.Entities.Comment;

namespace LinkUpPro.Domain.Interfaces.Repositories.Comment;

public interface ICommentRepository : IGenericRepository<Entities.Comment.Comment, Guid>
{
}