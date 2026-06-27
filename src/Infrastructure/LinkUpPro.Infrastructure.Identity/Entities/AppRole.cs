using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Infrastructure.Identity.Entities;

public class AppRole : IdentityRole<Guid>
{
    public AppRole() : base() { }

    public AppRole(string roleName) : base(roleName) { }
}
