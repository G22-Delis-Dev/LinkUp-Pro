using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Identity.Seeds;

public static class IdentitySeederExtensions
{
    public static async Task RunIdentitySeedsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            // Orden importa: primero roles, luego usuarios
            await DefaultRoles.SeedAsync(roleManager);
            await DefaultAdminUser.SeedAsync(userManager);
        }
        catch (Exception)
        {
            // Que rompa al iniciar si falta configuración crítica
            throw;
        }
    }
}
