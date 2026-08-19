using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Infrastructure.Shared.Helpers;

// Helper para validación de archivos subidos.
// Centraliza las reglas de validación del proyecto.
public static class FileValidationHelper
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedImageContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

    // Valida un archivo de imagen según las reglas del proyecto:
    // - Extensiones: .jpg, .jpeg, .png, .webp
    // - Tamaño máximo: 5 MB
    public static FileValidationResult ValidateImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return FileValidationResult.Fail("El archivo está vacío o no se proporcionó.");

        // Validar tamaño
        if (file.Length > MaxImageSizeBytes)
            return FileValidationResult.Fail(
                $"El archivo excede el tamaño máximo permitido de {MaxImageSizeBytes / 1024 / 1024} MB.");

        // Validar extensión
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
            return FileValidationResult.Fail(
                $"Extensión no permitida '{extension}'. Extensiones permitidas: {string.Join(", ", AllowedImageExtensions)}");

        // Validar content type
        var contentType = file.ContentType.ToLowerInvariant();
        if (!AllowedImageContentTypes.Contains(contentType))
            return FileValidationResult.Fail(
                $"Tipo de archivo no permitido. Tipos permitidos: {string.Join(", ", AllowedImageContentTypes)}");

        return FileValidationResult.Ok();
    }

    // Valida extensión de archivo sin necesidad del IFormFile completo.
    public static bool IsAllowedImageExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedImageExtensions.Contains(extension);
    }

    // Verifica si el tamaño está dentro del límite.
    public static bool IsWithinSizeLimit(long sizeInBytes)
    {
        return sizeInBytes > 0 && sizeInBytes <= MaxImageSizeBytes;
    }
}

// Resultado de la validación de archivos.
public class FileValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private FileValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static FileValidationResult Ok() => new(true);
    public static FileValidationResult Fail(string errorMessage) => new(false, errorMessage);
}
