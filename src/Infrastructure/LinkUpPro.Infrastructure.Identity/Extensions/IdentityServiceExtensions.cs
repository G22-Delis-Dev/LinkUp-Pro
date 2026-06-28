using LinkUpPro.Infrastructure.Identity.Context;
using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Identity.Extensions;

// Configuración modular de Identity siguiendo la guía del profesor (Video 1): Bloques de Lego
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── IdentityContext (esquema "Identity" separado) ─────────────
        services.AddDbContext<IdentityContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(
                    typeof(IdentityContext).Assembly.FullName)));

        // ── Identity Core + Roles + SignInManager ────────────────────
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // 1. Bloqueo por Fuerza Bruta (5 intentos, 15 min)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // 2. Complejidad de Contraseña Estricta
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;

            // 3. Configuración de usuario
            options.User.RequireUniqueEmail = true;

            // 4. Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // ── Vigencia de Tokens (24h activación, 1h reset) ───────────
        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(24);
        });

        return services;
    }

    // Método de extensión para ejecutar los Seeds en el arranque (Video 2)
    public static async Task RunIdentitySeedsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

            await Seeds.DefaultRoles.SeedAsync(roleManager);
            await Seeds.DefaultAdminUser.SeedAsync(userManager);
        }
        catch (Exception)
        {
            // Que rompa al iniciar si falta configuración crítica
            throw;
        }
    }
}