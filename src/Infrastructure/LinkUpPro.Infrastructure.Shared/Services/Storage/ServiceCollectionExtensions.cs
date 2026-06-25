using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra el servicio de almacenamiento de imágenes local
    /// </summary>
    public static IServiceCollection AddLocalImageStorage(this IServiceCollection services)
    {
        services.AddSingleton<IImageStorageService, LocalImageStorageService>();
        return services;
    }
}
