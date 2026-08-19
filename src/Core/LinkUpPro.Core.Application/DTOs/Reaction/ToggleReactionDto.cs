using LinkUpPro.Domain.Enums.Reaction;

namespace LinkUpPro.Application.DTOs.Reaction;

public class ToggleReactionDto
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
}
