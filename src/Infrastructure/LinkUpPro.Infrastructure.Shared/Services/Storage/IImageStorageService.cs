namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public interface IImageStorageService
{
    /// <summary>
    /// Guarda una imagen y retorna el path relativo donde fue guardada.
    /// Valida formato, contenido y tamaño antes de guardar.
    /// </summary>
    /// <param name="imageStream">Stream de la imagen</param>
    /// <param name="contentType">Tipo de contenido (image/jpeg, image/png, image/webp)</param>
    /// <param name="fileName">Nombre original del archivo (opcional, usado para validación adicional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Path relativo de la imagen guardada</returns>
    /// <exception cref="InvalidImageException">Si la imagen no pasa las validaciones</exception>
    Task<string> SaveImageAsync(Stream imageStream, string contentType, string? fileName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una imagen del almacenamiento
    /// </summary>
    /// <param name="imagePath">Path relativo de la imagen a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el stream de una imagen
    /// </summary>
    /// <param name="imagePath">Path relativo de la imagen</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Stream de la imagen o null si no existe</returns>
    Task<Stream?> GetImageAsync(string imagePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si una imagen existe
    /// </summary>
    /// <param name="imagePath">Path relativo de la imagen</param>
    /// <returns>True si existe, false si no</returns>
    Task<bool> ImageExistsAsync(string imagePath);

    /// <summary>
    /// Obtiene la URL pública de una imagen
    /// </summary>
    /// <param name="imagePath">Path relativo de la imagen</param>
    /// <returns>URL pública de la imagen</returns>
    string GetImageUrl(string imagePath);

    /// <summary>
    /// Valida una imagen sin guardarla
    /// </summary>
    /// <param name="imageStream">Stream de la imagen</param>
    /// <param name="contentType">Tipo de contenido</param>
    /// <param name="fileName">Nombre del archivo (opcional)</param>
    /// <returns>Resultado de la validación</returns>
    ImageValidationResult ValidateImage(Stream imageStream, string contentType, string? fileName = null);
}
