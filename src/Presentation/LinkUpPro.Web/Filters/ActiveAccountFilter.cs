using LinkUpPro.Domain.Interfaces.Repositories.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LinkUpPro.Infrastructure.Identity.Entities;

namespace LinkUpPro.Web.Filters;

/// <summary>
/// Filtro global que valida en cada request que:
/// 1. El usuario existe en Identity.
/// 2. La cuenta de dominio esté activa (IsActive = true).
/// Las cuentas inexistentes o inactivas se cierran sesión automáticamente.
/// </summary>
public class ActiveAccountFilter : IAsyncActionFilter
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ActiveAccountFilter> _logger;

    public ActiveAccountFilter(
        SignInManager<AppUser> signInManager,
        IUserRepository userRepository,
        ILogger<ActiveAccountFilter> logger)
    {
        _signInManager = signInManager;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            // 1. Verificar que exista en Identity
            var appUser = await _signInManager.UserManager.GetUserAsync(context.HttpContext.User);

            if (appUser == null)
            {
                _logger.LogWarning("Usuario autenticado no encontrado en Identity. Se cierra la sesión.");
                await ForceSignOut(context, "Su sesión ha expirado. Por favor, inicie sesión nuevamente.");
                return;
            }

            // 2. Verificar IsActive en el dominio usando el claim "uid"
            var uidClaim = context.HttpContext.User.FindFirst("uid")?.Value;
            if (Guid.TryParse(uidClaim, out var domainUserId))
            {
                var domainUser = await _userRepository.GetByIdAsync(domainUserId);

                if (domainUser == null || !domainUser.IsActive)
                {
                    _logger.LogWarning(
                        "Cuenta de dominio inactiva o no encontrada para uid={UserId}. Se cierra la sesión.",
                        domainUserId);
                    await ForceSignOut(context, "Su cuenta ha sido desactivada. Contacte al administrador.");
                    return;
                }
            }
        }

        await next();
    }

    private async Task ForceSignOut(ActionExecutingContext context, string message)
    {
        await _signInManager.SignOutAsync();
        context.Result = new RedirectToActionResult(
            "Login",
            "Auth",
            new { message });
    }
}
