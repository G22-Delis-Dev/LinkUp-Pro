using Microsoft.Extensions.Configuration;

namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public class LocalImageStorageService : IImageStorageService
{
    private readonly string _imagesBasePath;
    private readonly string _baseUrl;

    public LocalImageStorageService(IConfiguration configuration)
    {
        // Lee la ruta base desde configuración o usa valor por defecto
        // La ruta debe ser absoluta o relativa al directorio de ejecución
        _imagesBasePath = configuration["Storage:ImagesPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");
        
        // Asegura que el directorio existe
        if (!Directory.Exists(_imagesBasePath))
        {
            Directory.CreateDirectory(_imagesBasePath);
        }

        // URL base para acceder a las imágenes
        _baseUrl = configuration["Storage:BaseUrl"] ?? "/uploads/images";
    }

    public async Task<string> SaveImageAsync(Stream imageStream, string contentType, string? fileName = null, CancellationToken cancellationToken = default)
    {
        // Validar la imagen antes de guardarla
        var validationResult = ImageValidator.Validate(imageStream, contentType, fileName);
        if (!validationResult.IsValid)
        {
            throw new InvalidImageException(validationResult.ErrorMessage!);
        }

        // Genera un nombre único usando Guid
        var imageId = Guid.NewGuid();
        var extension = GetExtensionFromContentType(contentType);
        var fileName2 = $"{imageId}{extension}";
        
        // Organiza en subcarpetas por año/mes para mejor organización
        var now = DateTime.UtcNow;
        var relativePath = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"));
        var fullDirectoryPath = Path.Combine(_imagesBasePath, relativePath);
        
        // Asegura que el directorio existe
        if (!Directory.Exists(fullDirectoryPath))
        {
            Directory.CreateDirectory(fullDirectoryPath);
        }

        var fullFilePath = Path.Combine(fullDirectoryPath, fileName2);
        
        // Resetear el stream al inicio
        imageStream.Seek(0, SeekOrigin.Begin);
        
        // Guarda el archivo
        using (var fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
        {
            await imageStream.CopyToAsync(fileStream, cancellationToken);
        }

        // Retorna el path relativo (año/mes/guid.ext)
        return Path.Combine(relativePath, fileName2).Replace("\\", "/");
    }

    public Task DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return Task.CompletedTask;

        var fullPath = Path.Combine(_imagesBasePath, imagePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public async Task<Stream?> GetImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return null;

        var fullPath = Path.Combine(_imagesBasePath, imagePath);
        
        if (!File.Exists(fullPath))
            return null;

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> ImageExistsAsync(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return Task.FromResult(false);

        var fullPath = Path.Combine(_imagesBasePath, imagePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public string GetImageUrl(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return string.Empty;

        // Normaliza las barras para URLs
        var normalizedPath = imagePath.Replace("\\", "/");
        return $"{_baseUrl}/{normalizedPath}";
    }

    public ImageValidationResult ValidateImage(Stream imageStream, string contentType, string? fileName = null)
    {
        return ImageValidator.Validate(imageStream, contentType, fileName);
    }

    private static string GetExtensionFromContentType(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" => ".jpg",
            "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg" // Default a jpg
        };
    }
}
