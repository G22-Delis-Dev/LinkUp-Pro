namespace LinkUpPro.Infrastructure.Shared.Settings;

/// <summary>
/// Configuración de almacenamiento de imágenes.
/// Se bindea desde appsettings.json sección "StorageSettings".
/// </summary>
public class StorageSettings
{
    public string ImagesPath { get; set; } = "wwwroot/uploads/images";
    public string BaseUrl { get; set; } = "/uploads/images";
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024; // 5 MB
    public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".webp" };
}
