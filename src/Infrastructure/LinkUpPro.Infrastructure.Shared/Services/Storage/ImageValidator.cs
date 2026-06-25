namespace LinkUpPro.Infrastructure.Shared.Services.Storage;

public class ImageValidator
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private static readonly string[] AllowedContentTypes = 
    { 
        "image/jpeg", 
        "image/jpg", 
        "image/png", 
        "image/webp" 
    };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    // Firmas mágicas (magic numbers) para validar el contenido real del archivo
    private static readonly Dictionary<string, byte[][]> MagicNumbers = new()
    {
        {
            "image/jpeg", new[]
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, // JPEG JFIF
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, // JPEG EXIF
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }  // JPEG SPIFF
            }
        },
        {
            "image/png", new[]
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
            }
        },
        {
            "image/webp", new[]
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF (primeros 4 bytes, luego debe tener WEBP en offset 8-11)
            }
        }
    };

    public static ImageValidationResult Validate(Stream imageStream, string contentType, string? fileName = null)
    {
        // 1. Validar que el stream no esté vacío
        if (imageStream == null || imageStream.Length == 0)
        {
            return ImageValidationResult.Fail("El archivo está vacío");
        }

        // 2. Validar tamaño
        if (imageStream.Length > MaxFileSize)
        {
            return ImageValidationResult.Fail($"El archivo excede el tamaño máximo permitido de {MaxFileSize / 1024 / 1024} MB");
        }

        // 3. Validar content type
        if (string.IsNullOrWhiteSpace(contentType) || !AllowedContentTypes.Contains(contentType.ToLowerInvariant()))
        {
            return ImageValidationResult.Fail($"Tipo de archivo no permitido. Tipos permitidos: {string.Join(", ", AllowedContentTypes)}");
        }

        // 4. Validar extensión si se proporciona el nombre del archivo
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return ImageValidationResult.Fail($"Extensión no permitida. Extensiones permitidas: {string.Join(", ", AllowedExtensions)}");
            }
        }

        // 5. Validar el contenido real del archivo (magic numbers)
        var contentValidation = ValidateFileContent(imageStream, contentType);
        if (!contentValidation.IsValid)
        {
            return contentValidation;
        }

        return ImageValidationResult.Success();
    }

    private static ImageValidationResult ValidateFileContent(Stream imageStream, string contentType)
    {
        var normalizedContentType = contentType.ToLowerInvariant();
        
        // Normalizar jpeg/jpg
        if (normalizedContentType == "image/jpg")
        {
            normalizedContentType = "image/jpeg";
        }

        if (!MagicNumbers.ContainsKey(normalizedContentType))
        {
            return ImageValidationResult.Fail("Tipo de contenido no reconocido");
        }

        var position = imageStream.Position;
        imageStream.Seek(0, SeekOrigin.Begin);

        try
        {
            var signatures = MagicNumbers[normalizedContentType];
            var buffer = new byte[16]; // Suficiente para leer las firmas más largas
            var bytesRead = imageStream.Read(buffer, 0, buffer.Length);

            if (bytesRead < 4)
            {
                return ImageValidationResult.Fail("El archivo es demasiado pequeño para ser una imagen válida");
            }

            // Validación especial para WebP
            if (normalizedContentType == "image/webp")
            {
                // WebP debe tener "RIFF" al inicio y "WEBP" en los bytes 8-11
                if (bytesRead >= 12)
                {
                    var riff = buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46;
                    var webp = buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
                    
                    if (riff && webp)
                    {
                        return ImageValidationResult.Success();
                    }
                }
                return ImageValidationResult.Fail("El archivo no es un WebP válido");
            }

            // Validar contra las firmas conocidas
            foreach (var signature in signatures)
            {
                if (bytesRead >= signature.Length)
                {
                    var matches = true;
                    for (int i = 0; i < signature.Length; i++)
                    {
                        if (buffer[i] != signature[i])
                        {
                            matches = false;
                            break;
                        }
                    }

                    if (matches)
                    {
                        return ImageValidationResult.Success();
                    }
                }
            }

            return ImageValidationResult.Fail("El contenido del archivo no coincide con el tipo declarado. Posible intento de suplantación de tipo de archivo");
        }
        finally
        {
            imageStream.Seek(position, SeekOrigin.Begin);
        }
    }
}

public class ImageValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ImageValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ImageValidationResult Success() => new(true);
    public static ImageValidationResult Fail(string errorMessage) => new(false, errorMessage);
}
