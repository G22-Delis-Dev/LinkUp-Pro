using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.User;

namespace LinkUpPro.Domain.Entities.User;
public class UserToken : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public TokenType Type { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }
    public User User { get; set; } = null!;
}