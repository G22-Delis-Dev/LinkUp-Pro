using LinkUpPro.Domain.Enums.Reaction;

namespace LinkUpPro.Application.DTOs.Reaction;

public class ReactionDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string AuthorName { get; set; } = null!;
    public ReactionType Type { get; set; }
}
