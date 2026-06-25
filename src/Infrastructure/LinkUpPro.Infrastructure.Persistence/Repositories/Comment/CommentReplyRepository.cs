using LinkUpPro.Domain.Entities.Comment;
using LinkUpPro.Domain.Interfaces.Repositories.Comment;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class CommentReplyRepository : GenericRepository<CommentReply, Guid>, ICommentReplyRepository
    {
        public CommentReplyRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<CommentReply>> GetByCommentIdAsync(Guid commentId)
            => await _dbSet
                   .IgnoreQueryFilters() // Incluye eliminadas para mantener hilo
                   .Where(r => r.CommentId == commentId)
                   .OrderBy(r => r.CreatedAt)
                   .ToListAsync();

        public async Task<bool> HasChildRepliesAsync(Guid replyId)
            => await Task.FromResult(false);
    }

}
