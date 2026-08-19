using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LinkUpPro.Infrastructure.Identity.Services;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        
        // El claim "uid" se utiliza en la aplicación para referenciar el Id del usuario en el Dominio
        identity.AddClaim(new Claim("uid", user.UserId.ToString()));
        
        return identity;
    }
}
