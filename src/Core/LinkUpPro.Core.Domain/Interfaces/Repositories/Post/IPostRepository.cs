using System;
using LinkUpPro.Domain.Entities.Post;

namespace LinkUpPro.Domain.Interfaces.Repositories.Post;

public interface IPostRepository : IGenericRepository<Entities.Post.Post, Guid>
{
}