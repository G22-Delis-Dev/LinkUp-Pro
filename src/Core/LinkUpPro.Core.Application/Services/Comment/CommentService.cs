using AutoMapper;
using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Comment;
using LinkUpPro.Application.Interfaces.Comment;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Comment;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Comment;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationDispatchService _notificationDispatch;
    private readonly IMapper _mapper;

    public CommentService(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        INotificationDispatchService notificationDispatch,
        IMapper mapper)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _notificationDispatch = notificationDispatch;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto)
    {
        var post = await _postRepository.GetByIdAsync(dto.PostId);
        if (post == null)
            return ServiceResponse<CommentDto>.Failure("Publicación no encontrada.");

        if (!post.AllowComments)
            return ServiceResponse<CommentDto>.Failure("Esta publicación no permite comentarios.");

        var comment = new Domain.Entities.Comment.Comment
        {
            PostId = dto.PostId,
            UserId = dto.UserId,
            Content = dto.Content.Trim()
        };

        await _commentRepository.AddAsync(comment);

        var user = await _userRepository.GetByIdAsync(dto.UserId);

        // Notificar al autor del post (si no es el mismo que comenta)
        if (post.UserId != dto.UserId)
        {
            await _notificationDispatch.SendNotificationAsync(
                post.UserId,
                NotificationType.NewComment,
                $"{user?.FirstName} {user?.LastName} comentó en tu publicación.",
                comment.Id.ToString());
        }

        var result = new CommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            UserId = comment.UserId,
            AuthorName = user != null ? $"{user.FirstName} {user.LastName}" : "Usuario",
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ReplyCount = 0
        };

        return ServiceResponse<CommentDto>.Success(result);
    }

    public async Task<BaseResult> UpdateCommentAsync(Guid commentId, Guid userId, string newContent)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            return BaseResult.Fail("Comentario no encontrado.");

        if (comment.UserId != userId)
            return BaseResult.Fail("No tienes permisos para editar este comentario.");

        if (string.IsNullOrWhiteSpace(newContent))
            return BaseResult.Fail("El comentario no puede estar vacío.");

        comment.Content = newContent.Trim();
        await _commentRepository.UpdateAsync(comment);

        return BaseResult.Ok();
    }

    public async Task<BaseResult> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            return BaseResult.Fail("Comentario no encontrado.");

        if (comment.UserId != userId)
            return BaseResult.Fail("No tienes permisos para eliminar este comentario.");

        await _commentRepository.DeleteAsync(comment);
        return BaseResult.Ok();
    }

    public async Task<List<CommentDto>> GetCommentsByPostAsync(Guid postId)
    {
        var comments = await _commentRepository.Query()
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ToListAsync();

        return _mapper.Map<List<CommentDto>>(comments);
    }
}
