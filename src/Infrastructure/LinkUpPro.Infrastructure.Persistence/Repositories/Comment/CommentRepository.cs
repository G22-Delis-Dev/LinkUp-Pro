using LinkUpPro.Domain.Interfaces.Repositories.Comment;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : GenericRepository<Comment, Guid>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Comment>> GetByPostIdAsync(Guid postId)
            => await _dbSet
                   .Where(c => c.PostId == postId)
                   .Include(c => c.Replies)
                   .OrderBy(c => c.CreatedAt)
                   .ToListAsync();

        // Incluye todos los comentarios del post
        public async Task<IReadOnlyList<Comment>> GetByPostIncludingDeletedAsync(Guid postId)
            => await _dbSet
                   .Where(c => c.PostId == postId)
                   .Include(c => c.Replies)
                   .OrderBy(c => c.CreatedAt)
                   .ToListAsync();

        public async Task<int> CountByPostAsync(Guid postId)
            => await _dbSet
                   .CountAsync(c => c.PostId == postId);
    }
}
