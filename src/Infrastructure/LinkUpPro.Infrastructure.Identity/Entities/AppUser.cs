using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Infrastructure.Identity.Entities;

// Cuenta de autenticación (Identity). FirstName, LastName e IsActive duplicados para sesión/seeding.
public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool IsActive { get; set; } = false;

    // FK débil hacia la entidad User del dominio
    public Guid UserId { get; set; }
}