using LinkUpPro.Domain.Interfaces.Repositories.Reaction;
using LinkUpPro.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Domain.Entities.Reaction;
using LinkUpPro.Domain.Enums.Reaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUpPro.Infrastructure.Persistence.Repositories
{
    public class ReactionRepository : GenericRepository<Reaction, Guid>, IReactionRepository
    {
        public ReactionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Reaction?> GetByUserAndPostAsync(Guid userId, Guid postId)
            => await _dbSet
                   .FirstOrDefaultAsync(r =>
                       r.UserId == userId &&
                       r.PostId == postId);

        public async Task<(int Likes, int Dislikes)> GetCountsByPostAsync(Guid postId)
        {
            var likes = await _dbSet.CountAsync(r =>
                r.PostId == postId && r.Type == ReactionType.Like);
            var dislikes = await _dbSet.CountAsync(r =>
                r.PostId == postId && (r.Type == ReactionType.Sad || r.Type == ReactionType.Angry));
            return (likes, dislikes);
        }
    }
}
