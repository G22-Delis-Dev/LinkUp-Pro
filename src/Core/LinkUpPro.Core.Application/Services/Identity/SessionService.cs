using LinkUpPro.Application.Common;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Application.Services.Identity;

public class SessionService : ISessionService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public SessionService(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    // Invalida otras sesiones actualizando el SecurityStamp
    public async Task<BaseResult> InvalidateOtherSessionsAsync(Guid userId)
    {
        var appUser = await _userManager.FindByIdAsync(userId.ToString());
        if (appUser == null)
        {
            return BaseResult.Fail("Usuario no encontrado.");
        }

        await _userManager.UpdateSecurityStampAsync(appUser);

        // Re-login para que la sesión actual siga válida
        await _signInManager.RefreshSignInAsync(appUser);

        return BaseResult.Ok();
    }
}
