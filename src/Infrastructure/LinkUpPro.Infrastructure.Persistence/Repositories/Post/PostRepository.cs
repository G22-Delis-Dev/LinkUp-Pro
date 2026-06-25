using LinkUpPro.Domain.Enums.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class PostRepository : GenericRepository<Post, Guid>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<Post>> GetByAuthorAsync(
            Guid authorId, bool includeMedia = true)
        {
            var query = _dbSet
                .Where(p => p.UserId == authorId)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            if (includeMedia)
                query = query
                    .Include(p => p.Images)
                    .Include(p => p.Videos);

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<Post>> GetFriendsPostsAsync(
            IEnumerable<Guid> friendIds)
            => await _dbSet
                   .Where(p =>
                       friendIds.Contains(p.UserId) &&
                       p.Privacy == PostPrivacy.FriendsOnly)
                   .Include(p => p.Images)
                   .Include(p => p.Videos)
                   .OrderByDescending(p => p.CreatedAt)
                   .ToListAsync();

        public async Task<IReadOnlyList<Post>> SearchAsync(
            Guid authorId,
            string? text,
            PostContentType? contentType,
            DateTime? from,
            DateTime? to,
            bool? editedOnly)
        {
            var query = _dbSet
                .Where(p => p.UserId == authorId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(p =>
                    p.Content.ToLower().Contains(text.Trim().ToLower()));

            if (contentType.HasValue)
                query = query.Where(p => p.ContentType == contentType.Value);

            if (from.HasValue)
                query = query.Where(p => p.CreatedAt >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(p => p.CreatedAt < to.Value.Date.AddDays(1));

            if (editedOnly == true)
                query = query.Where(p => p.LastModifiedAt != null);
            else if (editedOnly == false)
                query = query.Where(p => p.LastModifiedAt == null);

            return await query
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post?> GetWithMediaAsync(Guid postId)
            => await _dbSet
                   .Include(p => p.Images)
                   .Include(p => p.Videos)
                   .FirstOrDefaultAsync(p => p.Id == postId);
    }
}
