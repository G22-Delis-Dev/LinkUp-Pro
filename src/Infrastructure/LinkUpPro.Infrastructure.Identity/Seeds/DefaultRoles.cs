using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Infrastructure.Identity.Seeds;

public static class DefaultRoles
{
    public static async Task SeedAsync(RoleManager<AppRole> roleManager)
    {
        // Roles del sistema
        var roles = new[] { "Admin", "User" };

        foreach (var roleName in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new AppRole(roleName));
            }
        }
    }
}
