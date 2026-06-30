using LinkUpPro.Application.Extensions;
using LinkUpPro.Infrastructure.Identity.Extensions;
using LinkUpPro.Infrastructure.Persistence.Extensions;
using LinkUpPro.Infrastructure.Shared.Extensions;
using LinkUpPro.Web.Filters;
using LinkUpPro.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════
// Registro de servicios por capas (Extension Methods)
// ═══════════════════════════════════════════════════════════════════
builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);

// ── Cookies de Autenticación ─────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie security settings
    options.Cookie.HttpOnly = true;         // No accesible desde JavaScript
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict;           // Protección CSRF

    // Sesión expira tras 30 minutos de inactividad
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // Renueva en cada request

    // Rutas de autenticación
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// ── Session ──────────────────────────────────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ── MVC + Filtros Globales ───────────────────────────────────────
builder.Services.AddControllersWithViews(options =>
{
    // Traducciones de validaciones implícitas de ASP.NET Core
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Este campo es requerido.");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(_ => "Se requiere un valor para este campo.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(_ => "El campo debe ser un número.");
    options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => $"El valor '{x}' no es válido.");
    options.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor provisto no es válido.");
    
    // Filtro global para validar cuentas activas
    options.Filters.Add<ActiveAccountFilter>();
});

builder.Services.AddScoped<ActiveAccountFilter>();

// ── Anti-forgery ─────────────────────────────────────────────────
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

// ═══════════════════════════════════════════════════════════════════
// Ejecutar Seeds de Identity (Video 2 del profe)
// ═══════════════════════════════════════════════════════════════════
await app.Services.RunIdentitySeedsAsync();

// ═══════════════════════════════════════════════════════════════════
// Pipeline HTTP
// ═══════════════════════════════════════════════════════════════════

// Error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Index");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdn.tailwindcss.com; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "connect-src 'self'; " +
        "frame-src https://www.youtube.com https://youtube.com; " +
        "frame-ancestors 'none';");

    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session debe ir antes de Authentication
app.UseSession();

// Authentication y Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
