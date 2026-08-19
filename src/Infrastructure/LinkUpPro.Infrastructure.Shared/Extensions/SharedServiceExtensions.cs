using LinkUpPro.Infrastructure.Shared.Services.Email;
using LinkUpPro.Infrastructure.Shared.Services.Storage;
using LinkUpPro.Infrastructure.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Shared.Extensions;

public static class SharedServiceExtensions
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ═══════════════════════════════════════════════════════════
        // 1. Bind Settings desde appsettings.json
        // ═══════════════════════════════════════════════════════════
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        // ═══════════════════════════════════════════════════════════
        // 2. Servicios (Scoped — comparten el ciclo del request HTTP)
        // ═══════════════════════════════════════════════════════════
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IImageStorageService, LocalImageStorageService>();

        return services;
    }
}
