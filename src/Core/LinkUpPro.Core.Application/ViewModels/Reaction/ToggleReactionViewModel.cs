using LinkUpPro.Domain.Enums.Reaction;

namespace LinkUpPro.Application.ViewModels.Reaction;

public class ToggleReactionViewModel
{
    public Guid PostId { get; set; }
    public ReactionType Type { get; set; }
}
