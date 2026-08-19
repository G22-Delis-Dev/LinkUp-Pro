using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Reaction;
using LinkUpPro.Domain.Enums.Reaction;

namespace LinkUpPro.Application.Interfaces.Reaction;

public interface IReactionService
{
    // Toggle: misma reacción = quitar, distinta = cambiar, nueva = crear
    Task<ServiceResponse<ReactionDto>> ToggleReactionAsync(ToggleReactionDto dto);
    Task<List<ReactionDto>> GetReactionsByPostAsync(Guid postId);
    Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid postId);
    Task<ReactionCountsResult> GetReactionCountsAsync(Guid postId, Guid userId);
}

public class ReactionCountsResult
{
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int UserReaction { get; set; } = -1; // -1 = ninguna, 0 = Like, 1 = Dislike
}
