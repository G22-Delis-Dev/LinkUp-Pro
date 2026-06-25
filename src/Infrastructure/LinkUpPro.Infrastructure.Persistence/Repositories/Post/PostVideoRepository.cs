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
    public class PostVideoRepository : GenericRepository<PostVideo, Guid>, IPostVideoRepository
    {
        public PostVideoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PostVideo?> GetByPostIdAsync(Guid postId)
            => await _dbSet.FirstOrDefaultAsync(v => v.PostId == postId);
    }
}
