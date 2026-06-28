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
}
