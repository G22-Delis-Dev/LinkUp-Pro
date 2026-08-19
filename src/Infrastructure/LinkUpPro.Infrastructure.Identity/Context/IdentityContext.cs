using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Infrastructure.Identity.Context;

/// <summary>
/// Contexto de base de datos exclusivo para Identity.
/// Separado del ApplicationDbContext siguiendo la guía del profesor:
/// "Aislar las tablas de credenciales en el esquema Identity para que los 
/// administradores de BD puedan restringir el acceso."
/// </summary>
public class IdentityContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Esquema propio para aislar tablas de credenciales
        builder.HasDefaultSchema("Identity");

        // 2. Renombramiento de tablas (evita redundancia tipo "Identity.IdentityRole")
        builder.Entity<AppUser>().ToTable("Users");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
