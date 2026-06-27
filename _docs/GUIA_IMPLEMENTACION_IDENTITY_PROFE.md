# 🎓 Guía Completa de Implementación de Identity (Estándares y Filosofía de Videos 1, 2, 3 y 4)

> [!IMPORTANT]
> **Objetivo:** Este documento compila el 100% de las enseñanzas, trucos arquitectónicos y buenas prácticas transmitidas por el profesor en sus 4 videos sobre **ASP.NET Core Identity**. Toda la implementación del equipo en **LinkUp Pro** debe seguir estrictamente estos patrones.

---

## 🏛️ 1. Filosofía Arquitectónica (Video 1 & 3)

### A. Encapsulamiento en `Infra.Identity` y Desacoplamiento Heurístico
* **Regla:** Todo lo relacionado con ASP.NET Core Identity (entidades de usuario, roles, contextos de base de datos de identidad, seeds y servicios de cuenta) debe vivir en su propia capa (`Infra.Identity`).
* **Por qué:** Para proteger el núcleo de la aplicación (`Domain` y `Application`). Si en el futuro la empresa migra la seguridad a **Auth0**, **Azure Active Directory** u **Okta**, el sistema solo reemplaza la capa de infraestructura sin alterar la lógica de negocio.

### B. Entidad `AppUser` vs `IdentityUser` (Video 1)
* **Cuenta vs Usuario:** Una cuenta (`IdentityUser`) sirve solo para autenticarse (email, username, hash de clave). Un usuario (`AppUser`) representa a la persona en el negocio (nombre, apellido, foto).
* **Regla:** Crear `AppUser : IdentityUser<Guid>` dentro de `Infra.Identity/Entities`. **Nunca meter `AppUser` en `Domain`**, ya que el dominio no debe tener dependencias de frameworks externos.

### C. Relaciones Débiles en el Negocio (Video 3)
* En el paso a Identity, las entidades del negocio (*Publicaciones, Comentarios, Amistades, Partidas de Battleship*) **no deben tener Navigation Properties directas de EF Core** hacia `AppUser`.
* **Regla:** Guardar únicamente el `UserId` (como `Guid` o `string`) en las entidades del negocio (*Relación Débil*). Las consultas de nombres y fotos de perfil se coordinan a través de los servicios (`IAccountServiceForWebApp` o `IUserService`).

---

## 🔒 2. Seguridad en Base de Datos (`OnModelCreating`) (Video 1)

Al configurar el `IdentityContext`, el profesor prohíbe dejar la generación por defecto y exige estándares profesionales de ciberseguridad y DBA:

### A. Esquema Propio (`HasDefaultSchema`)
```csharp
builder.HasDefaultSchema("Identity");
```
* **Por qué:** Aislar las tablas de credenciales en el esquema `Identity` (*ej. `Identity.Users`*) para que los administradores de BD puedan restringir el acceso a los desarrolladores normales y proteger la información sensible.

### B. Renombramiento de Tablas (`ToTable`)
Para evitar nombres redundantes como `Identity.IdentityRole`:
```csharp
builder.Entity<AppUser>().ToTable("Users");
builder.Entity<AppRole>().ToTable("Roles");
builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
```

---

## ⚙️ 3. Configuración Modular y Políticas de Seguridad (Video 1)

Construcción modular en `IdentityServiceExtensions.cs` como "bloques de Lego":

```csharp
services.AddIdentityCore<AppUser>(options => {
    // 1. Bloqueo por Fuerza Bruta (Brute Force)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // 2. Complejidad de Contraseña Estricta
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<AppRole>()
.AddSignInManager()
.AddEntityFrameworkStores<IdentityContext>()
.AddTokenProvider<DataProtectorTokenProvider<AppUser>>("Default");

// 3. Vigencia de Tokens (Activación y Reset)
services.Configure<DataProtectionTokenProviderOptions>(options => {
    options.TokenLifespan = TimeSpan.FromHours(24); // 24 horas máximo
});
```

### Ciclo de Vida de Inyecciones (`Scoped` vs `Transient`)
* **Contextos y Repositorios $\rightarrow$ `AddScoped`:** Comparten la transacción HTTP del request, evitando saturar conexiones simultáneas a SQL Server.
* **Helpers y Servicios Utilitarios sin Estado $\rightarrow$ `AddTransient`:** Solo para clases ligeras de apoyo.

---

## 🌱 4. Seeding Automatizado de Datos Iniciales (Video 2)

El sistema debe autoconstruirse la primera vez que corre sin necesidad de crear usuarios administradores manualmente en la base de datos.

### A. Clases de Seed en `Infra.Identity/Seeds/`
Crear clases estáticas como `DefaultRoles.SeedAsync(RoleManager<AppRole>)` y `DefaultAdminUser.SeedAsync(UserManager<AppUser>)`.
* **Validación de Existencia:** Antes de crear, verificar con `All()` o `FindByEmailAsync()` que el usuario/rol no exista para evitar duplicados en ejecuciones posteriores:
  ```csharp
  var existingUser = await userManager.FindByEmailAsync("admin@linkuppro.com");
  if (existingUser == null) {
      var admin = new AppUser {
          UserName = "admin",
          Email = "admin@linkuppro.com",
          EmailConfirmed = true,
          FirstName = "Admin",
          LastName = "LinkUp"
      };
      await userManager.CreateAsync(admin, "Admin123$!");
      await userManager.AddToRoleAsync(admin, "Admin");
  }
  ```

### B. Ejecución Limpia en `Program.cs` mediante Extension Method
El `Program.cs` web no debe tener lógica de creación de scopes engorrosa. Se debe crear un método de extensión en la capa de Identity:
```csharp
public static async Task RunIdentitySeedsAsync(this IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var services = scope.ServiceProvider;
    try {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        await DefaultRoles.SeedAsync(roleManager);
        await DefaultAdminUser.SeedAsync(userManager);
    }
    catch (Exception ex) {
        // Que rompa al iniciar si falta configuración crítica
        throw; 
    }
}
```
Y en `Program.cs` simplemente se llama después del build:
```csharp
var app = builder.Build();
await app.Services.RunIdentitySeedsAsync();
```

---

## 🚀 5. Optimización en Controladores y Vistas MVC (Video 3 & 4)

El profesor enseña cómo aprovechar al máximo las Cookies y Claims de Identity para hacer un código ultra limpio:

### A. Cero Manejo Manual de Sesión en Login (Video 3)
* Al autenticar al usuario mediante `SignInManager.PasswordSignInAsync`, Identity genera automáticamente la cookie cifrada con los Claims. **No se debe guardar al usuario en `HttpContext.Session` manualmente.**

### B. Verificación de Roles Directamente en Vistas Razor (Video 4)
* **Prohibido:** Pasar roles o banderas booleanas por `ViewBag` desde el controlador a cada vista para ocultar/mostrar botones.
* **Correcto:** Usar la propiedad `User` nativa de Razor que lee los claims de la cookie:
  ```html
  @if (User.IsInRole("Admin")) {
      <a href="/admin/config" class="btn btn-danger">Panel Admin</a>
  }
  ```

### C. Extracción de ID y Datos desde Claims en Controladores (Video 4)
* Cuando un usuario autenticado hace una acción (*ej. Crear Publicación, Atacar en Battleship*), **no hacer consultas a la BD (`UserManager.FindByNameAsync`) solo para saber su ID**.
* Extrayendo el ID directamente de la cookie de autenticación (Claims):
  ```csharp
  using System.Security.Claims;
  // En el controlador:
  var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
  ```
  Esto ahorra miles de llamadas innecesarias a SQL Server mejorando drásticamente el rendimiento (*Performance*).

---

## 📋 Checklist de Cumplimiento con el Profesor para LinkUp Pro

- [x] Capa `Infra.Identity` creada y separada del `Domain`.
- [x] Entidad `AppUser : IdentityUser<Guid>` creada con propiedades de persona.
- [ ] Esquema `"Identity"` y renombramiento de tablas (`Users`, `Roles`) en `OnModelCreating`.
- [ ] Opciones de `Lockout` configuradas en 5 intentos / 15 minutos.
- [ ] Opciones de `Password` configuradas en 8 chars + mayús + minús + núm + especial.
- [ ] Seeding automatizado de roles y usuarios implementado en `Extension Method`.
- [ ] Uso de `User.IsInRole()` en vistas Razor en lugar de `ViewBag`.
- [ ] Uso de `User.FindFirstValue(ClaimTypes.NameIdentifier)` en controladores para obtener el ID sin consultar BD.
