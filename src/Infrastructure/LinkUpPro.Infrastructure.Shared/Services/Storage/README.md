# Image Storage Service

Servicio para almacenamiento local de imágenes con validación integrada y nombres basados en GUID.

## Características

- **Validación robusta**: Valida formato, contenido (magic numbers) y tamaño
- **Formatos permitidos**: `.jpg`, `.jpeg`, `.png`, `.webp`
- **Tamaño máximo**: 5 MB
- **Nombres únicos**: Todas las imágenes se guardan con nombres basados en GUID
- **Sin nombre original**: No se preserva el nombre original del archivo por seguridad
- **Organización automática**: Las imágenes se organizan en carpetas por año/mes
- **Detección de falsificación**: Valida que el contenido real coincida con el tipo declarado
- **URLs públicas**: Generación automática de URLs para acceder a las imágenes

## Validaciones implementadas

### 1. Validación de tamaño
- Máximo: **5 MB**
- Se valida antes de guardar el archivo

### 2. Validación de formato
- Solo se permiten: **image/jpeg**, **image/png**, **image/webp**
- Se valida el content-type declarado

### 3. Validación de contenido (Magic Numbers)
- **JPEG**: Verifica firma `FF D8 FF` al inicio del archivo
- **PNG**: Verifica firma `89 50 4E 47 0D 0A 1A 0A`
- **WebP**: Verifica firma RIFF + WEBP en posiciones específicas
- Previene ataques de suplantación de tipo de archivo

### 4. Validación de extensión
- Si se proporciona el nombre del archivo, valida que la extensión sea permitida
- Extensiones: `.jpg`, `.jpeg`, `.png`, `.webp`

## Configuración

### 1. Registrar el servicio en `Program.cs`:

```csharp
using LinkUpPro.Infrastructure.Shared.Services.Storage;

// Registrar el servicio
builder.Services.AddLocalImageStorage();
```

### 2. Configurar en `appsettings.json`:

```json
{
  "Storage": {
    "ImagesPath": "C:\\path\\to\\your\\images",
    "BaseUrl": "/uploads/images"
  }
}
```

**Nota**: Si no se especifica, usa valores por defecto:
- `ImagesPath`: `{CurrentDirectory}/wwwroot/uploads/images`
- `BaseUrl`: `/uploads/images`

## Uso

### Ejemplo 1: Guardar una imagen con validación automática

```csharp
public class PostService
{
    private readonly IImageStorageService _imageStorage;

    public PostService(IImageStorageService imageStorage)
    {
        _imageStorage = imageStorage;
    }

    public async Task<string> CreatePostWithImageAsync(IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
            return null;

        try
        {
            using var stream = imageFile.OpenReadStream();
            
            // La validación se ejecuta automáticamente
            // Lanza InvalidImageException si no es válida
            var imagePath = await _imageStorage.SaveImageAsync(
                stream, 
                imageFile.ContentType,
                imageFile.FileName  // Opcional, para validación adicional
            );
            
            return imagePath;
        }
        catch (InvalidImageException ex)
        {
            // Manejar error de validación
            throw new BadRequestException($"Imagen inválida: {ex.Message}");
        }
    }
}
```

### Ejemplo 2: Validar imagen antes de guardarla

```csharp
public async Task<IActionResult> ValidateAndUpload(IFormFile imageFile)
{
    using var stream = imageFile.OpenReadStream();
    
    // Validar sin guardar
    var validationResult = _imageStorage.ValidateImage(
        stream, 
        imageFile.ContentType, 
        imageFile.FileName
    );
    
    if (!validationResult.IsValid)
    {
        return BadRequest(new { error = validationResult.ErrorMessage });
    }
    
    // Si es válida, guardar
    stream.Seek(0, SeekOrigin.Begin);
    var imagePath = await _imageStorage.SaveImageAsync(
        stream, 
        imageFile.ContentType,
        imageFile.FileName
    );
    
    return Ok(new { path = imagePath });
}
```

### Ejemplo 3: Manejo de errores de validación

```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadImage(IFormFile image)
{
    if (image == null)
        return BadRequest("No se proporcionó ninguna imagen");

    try
    {
        using var stream = image.OpenReadStream();
        var imagePath = await _imageStorage.SaveImageAsync(
            stream, 
            image.ContentType,
            image.FileName
        );
        
        var imageUrl = _imageStorage.GetImageUrl(imagePath);
        
        return Ok(new
        {
            success = true,
            path = imagePath,
            url = imageUrl
        });
    }
    catch (InvalidImageException ex)
    {
        return BadRequest(new 
        { 
            success = false, 
            error = ex.Message 
        });
    }
}
```

### Ejemplo 2: Obtener URL pública de una imagen

```csharp
public class PostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string ImagePath { get; set; }
    public string ImageUrl { get; set; }
}

public async Task<PostDto> GetPostAsync(Guid postId)
{
    var post = await _repository.GetByIdAsync(postId);
    
    return new PostDto
    {
        Id = post.Id,
        Content = post.Content,
        ImagePath = post.ImagePath,
        // Convierte el path a URL pública
        ImageUrl = _imageStorage.GetImageUrl(post.ImagePath)
        // Resultado: "/uploads/images/2024/12/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg"
    };
}
```

### Ejemplo 3: Eliminar una imagen

```csharp
public async Task DeletePostAsync(Guid postId)
{
    var post = await _repository.GetByIdAsync(postId);
    
    if (!string.IsNullOrEmpty(post.ImagePath))
    {
        // Elimina la imagen física del disco
        await _imageStorage.DeleteImageAsync(post.ImagePath);
    }
    
    await _repository.DeleteAsync(post);
}
```

### Ejemplo 4: Actualizar imagen de un post

```csharp
public async Task UpdatePostImageAsync(Guid postId, IFormFile newImageFile)
{
    var post = await _repository.GetByIdAsync(postId);
    
    // Si ya tiene una imagen, elimínala
    if (!string.IsNullOrEmpty(post.ImagePath))
    {
        await _imageStorage.DeleteImageAsync(post.ImagePath);
    }
    
    // Guarda la nueva imagen
    using var stream = newImageFile.OpenReadStream();
    post.ImagePath = await _imageStorage.SaveImageAsync(
        stream, 
        newImageFile.ContentType
    );
    
    await _repository.UpdateAsync(post);
}
```

### Ejemplo 5: Servir imágenes desde un endpoint

```csharp
[HttpGet("images/{year}/{month}/{filename}")]
public async Task<IActionResult> GetImage(string year, string month, string filename)
{
    var imagePath = $"{year}/{month}/{filename}";
    
    var stream = await _imageStorage.GetImageAsync(imagePath);
    
    if (stream == null)
        return NotFound();
    
    // Determina el content type según la extensión
    var contentType = filename.EndsWith(".png") ? "image/png" : "image/jpeg";
    
    return File(stream, contentType);
}
```

## Estructura de archivos

Las imágenes se guardan con la siguiente estructura:

```
wwwroot/
└── uploads/
    └── images/
        ├── 2024/
        │   ├── 11/
        │   │   ├── a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg
        │   │   └── f1e2d3c4-b5a6-9807-bcde-fg2345678901.png
        │   └── 12/
        │       └── ...
        └── 2025/
            └── 01/
                └── ...
```

## Mensajes de error de validación

El servicio proporciona mensajes claros cuando una imagen no es válida:

- `"El archivo está vacío"`
- `"El archivo excede el tamaño máximo permitido de 5 MB"`
- `"Tipo de archivo no permitido. Tipos permitidos: image/jpeg, image/png, image/webp"`
- `"Extensión no permitida. Extensiones permitidas: .jpg, .jpeg, .png, .webp"`
- `"El archivo es demasiado pequeño para ser una imagen válida"`
- `"El contenido del archivo no coincide con el tipo declarado. Posible intento de suplantación de tipo de archivo"`

## Content Types soportados

- `image/jpeg` → `.jpg`
- `image/jpg` → `.jpg`
- `image/png` → `.png`
- `image/webp` → `.webp`

## Validación de imágenes (incluida)

El servicio incluye validación integrada, pero también puedes usar el método `ValidateImage` para validar sin guardar:

```csharp
var validationResult = _imageStorage.ValidateImage(stream, contentType, fileName);
if (!validationResult.IsValid)
{
    Console.WriteLine(validationResult.ErrorMessage);
}
```

## Notas de seguridad

- ✅ **Validación de contenido real**: Usa magic numbers para verificar que el archivo sea realmente una imagen
- ✅ **Prevención de suplantación**: Valida que el contenido coincida con el tipo declarado
- ✅ **Límite de tamaño**: Máximo 5 MB por archivo
- ✅ **Formatos restringidos**: Solo JPEG, PNG y WebP
- ✅ **Nombres únicos con GUID**: Evita inyección de rutas y conflictos
- ✅ **Sin nombre original**: No se preserva información del usuario
- ✅ **Extensión por content-type**: No confía en la extensión del archivo original

## Ejemplos de ataques prevenidos

### 1. Suplantación de tipo de archivo
```
Ataque: Subir un archivo .exe renombrado como .jpg
Resultado: ❌ Rechazado - El contenido no coincide con image/jpeg
```

### 2. Archivo muy grande (DoS)
```
Ataque: Subir un archivo de 100 MB
Resultado: ❌ Rechazado - Excede el tamaño máximo de 5 MB
```

### 3. Tipo de archivo no permitido
```
Ataque: Subir un SVG (puede contener scripts)
Resultado: ❌ Rechazado - Tipo no permitido
```
