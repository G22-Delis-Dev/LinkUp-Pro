using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Infrastructure.Identity.Entities;

public class AppUser : IdentityUser<Guid>
{
    public Guid UserId { get; set; }
}