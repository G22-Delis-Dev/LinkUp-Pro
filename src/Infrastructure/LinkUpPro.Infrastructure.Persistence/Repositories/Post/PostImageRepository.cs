using LinkUpPro.Domain.Entities.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class PostImageRepository : GenericRepository<PostImage, Guid>, IPostImageRepository
    {
        public PostImageRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PostImage?> GetByPostIdAsync(Guid postId)
            => await _dbSet.FirstOrDefaultAsync(i => i.PostId == postId);
    }
}
