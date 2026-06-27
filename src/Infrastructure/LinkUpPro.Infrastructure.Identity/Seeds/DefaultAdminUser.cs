using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Infrastructure.Identity.Seeds;

public static class DefaultAdminUser
{
    public static async Task SeedAsync(UserManager<AppUser> userManager)
    {
        // Verificar si ya existe el admin para evitar duplicados
        var existingUser = await userManager.FindByEmailAsync("admin@linkuppro.com");
        if (existingUser != null)
            return;

        var admin = new AppUser
        {
            UserName = "admin",
            Email = "admin@linkuppro.com",
            EmailConfirmed = true,          // Admin no requiere activación
            FirstName = "Admin",
            LastName = "LinkUp",
            IsActive = true,
            UserId = Guid.Empty             // Se actualizará cuando se cree el User en Domain
        };

        var result = await userManager.CreateAsync(admin, "Admin123$!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
