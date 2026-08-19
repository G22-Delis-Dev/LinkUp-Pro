using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public static class ServiceCollectionExtensions
{
    // Registra el servicio de almacenamiento de imágenes local
    public static IServiceCollection AddLocalImageStorage(this IServiceCollection services)
    {
        services.AddSingleton<IImageStorageService, LocalImageStorageService>();
        return services;
    }
}
