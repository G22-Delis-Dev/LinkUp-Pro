using LinkUpPro.Domain.Enums.Post;
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.ViewModels.Post;

public class CreatePostViewModel
{
    public string Content { get; set; } = null!;
    public PostPrivacy Privacy { get; set; } = PostPrivacy.FriendsOnly;
    public bool AllowComments { get; set; } = true;

    // Tipo de contenido: Imagen o Video (toggle dinámico en la vista)
    public PostContentType ContentType { get; set; }

    // Archivo de imagen (cuando ContentType = Image)
    public IFormFile? Image { get; set; }

    // URL de YouTube (cuando ContentType = Video)
    public string? YouTubeUrl { get; set; }
}
