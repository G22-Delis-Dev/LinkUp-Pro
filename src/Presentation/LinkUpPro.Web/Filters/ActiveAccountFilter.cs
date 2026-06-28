using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authentication;
using LinkUpPro.Infrastructure.Identity.Entities;

namespace LinkUpPro.Web.Filters;

// Filtro que valida si los usuarios autenticados tienen cuentas activas.
// Las cuentas inactivas se cierran sesión automáticamente.
public class ActiveAccountFilter : IAsyncActionFilter
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<ActiveAccountFilter> _logger;

    public ActiveAccountFilter(
        SignInManager<AppUser> signInManager,
        ILogger<ActiveAccountFilter> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Only validate if the user is authenticated
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var appUser = await _signInManager.UserManager.GetUserAsync(context.HttpContext.User);

            // If Identity user not found, sign out and redirect
            if (appUser == null)
            {
                _logger.LogWarning("Authenticated user not found in Identity system");
                
                // Sign out the user
                await _signInManager.SignOutAsync();

                // Redirect to login with message
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Auth",
                    new { message = "Su sesión ha expirado. Por favor, inicie sesión nuevamente." }
                );
                return;
            }

            // TODO: Additional validation with domain User.IsActive
            // should be implemented when integrating with domain services:
            // var domainUser = await _userRepository.GetByIdAsync(userId);
            // if (domainUser == null || !domainUser.IsActive) { ... }
        }

        // Continue with the action execution
        await next();
    }
}
