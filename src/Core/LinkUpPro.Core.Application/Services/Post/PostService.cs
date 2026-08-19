using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Post;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Domain.Entities.Post;
using LinkUpPro.Domain.Enums.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Shared.Helpers;
using LinkUpPro.Infrastructure.Shared.Services.Storage;

namespace LinkUpPro.Application.Services.Post;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostImageRepository _postImageRepository;
    private readonly IPostVideoRepository _postVideoRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImageStorageService _imageStorage;

    public PostService(
        IPostRepository postRepository,
        IPostImageRepository postImageRepository,
        IPostVideoRepository postVideoRepository,
        IUserRepository userRepository,
        IImageStorageService imageStorage)
    {
        _postRepository = postRepository;
        _postImageRepository = postImageRepository;
        _postVideoRepository = postVideoRepository;
        _userRepository = userRepository;
        _imageStorage = imageStorage;
    }

    public async Task<ServiceResponse<PostDto>> CreatePostAsync(CreatePostDto dto)
    {
        // Validar que tiene texto + media (prohibido solo texto, solo imagen, solo video)
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            return ServiceResponse<PostDto>.Failure("El post debe tener contenido de texto.");
        }

        var hasImage = dto.ImageStream != null;
        var hasVideo = !string.IsNullOrWhiteSpace(dto.YouTubeUrl);

        if (!hasImage && !hasVideo)
        {
            return ServiceResponse<PostDto>.Failure(
                "El post debe incluir una imagen o un enlace de video de YouTube además del texto.");
        }

        if (hasImage && hasVideo)
        {
            return ServiceResponse<PostDto>.Failure(
                "El post no puede tener imagen y video al mismo tiempo.");
        }

        // Determinar el tipo de contenido
        var contentType = hasImage ? PostContentType.Image : PostContentType.Video;

        // Crear el post
        var post = new Domain.Entities.Post.Post
        {
            UserId = dto.UserId,
            Content = dto.Content.Trim(),
            Privacy = dto.Privacy,
            ContentType = contentType,
            AllowComments = dto.AllowComments
        };

        await _postRepository.AddAsync(post);

        // Guardar media
        if (hasImage)
        {
            var imagePath = await _imageStorage.SaveImageAsync(
                dto.ImageStream!, dto.ImageContentType!, dto.ImageFileName);

            await _postImageRepository.AddAsync(new PostImage
            {
                PostId = post.Id,
                ImagePath = imagePath
            });
        }
        else if (hasVideo)
        {
            var videoId = YouTubeHelper.ExtractVideoId(dto.YouTubeUrl!);
            if (string.IsNullOrEmpty(videoId))
            {
                return ServiceResponse<PostDto>.Failure(
                    "La URL de YouTube no es válida.");
            }

            await _postVideoRepository.AddAsync(new PostVideo
            {
                PostId = post.Id,
                VideoPath = videoId // Guardamos el ID del video, no la URL completa
            });
        }

        // Obtener el user para armar el DTO de respuesta
        var user = await _userRepository.GetByIdAsync(dto.UserId);

        var resultDto = new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            AuthorName = user != null ? $"{user.FirstName} {user.LastName}" : "Usuario",
            Content = post.Content,
            Privacy = post.Privacy,
            ContentType = post.ContentType,
            AllowComments = post.AllowComments,
            ImageUrl = hasImage ? _imageStorage.GetImageUrl(post.Images.FirstOrDefault()?.ImagePath ?? "") : null,
            YouTubeVideoId = hasVideo ? YouTubeHelper.ExtractVideoId(dto.YouTubeUrl!) : null,
            CreatedAt = post.CreatedAt
        };

        return ServiceResponse<PostDto>.Success(resultDto);
    }

    public async Task<BaseResult> UpdatePostAsync(UpdatePostDto dto)
    {
        var post = await _postRepository.GetByIdAsync(dto.PostId);
        if (post == null)
        {
            return BaseResult.Fail("Publicación no encontrada.");
        }

        // Solo el autor puede editar
        if (post.UserId != dto.UserId)
        {
            return BaseResult.Fail("No tienes permisos para editar esta publicación.");
        }

        post.Content = dto.Content.Trim();
        post.Privacy = dto.Privacy;
        post.AllowComments = dto.AllowComments;

        await _postRepository.UpdateAsync(post);

        return BaseResult.Ok();
    }

    public async Task<BaseResult> DeletePostAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null)
        {
            return BaseResult.Fail("Publicación no encontrada.");
        }

        // Solo el autor puede eliminar
        if (post.UserId != userId)
        {
            return BaseResult.Fail("No tienes permisos para eliminar esta publicación.");
        }

        // Eliminación lógica usando el campo IsDeleted o cambiando el estado
        // Por ahora usamos eliminación física ya que no hay campo IsDeleted en la entidad
        await _postRepository.DeleteAsync(post);

        return BaseResult.Ok();
    }
}
