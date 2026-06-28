using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Comment;
using LinkUpPro.Application.Interfaces.Comment;
using LinkUpPro.Domain.Interfaces.Repositories.Comment;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Comment;

public class CommentReplyService : ICommentReplyService
{
    private readonly ICommentReplyRepository _replyRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentReplyService(
        ICommentReplyRepository replyRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        _replyRepository = replyRepository;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResponse<CommentReplyDto>> CreateReplyAsync(CreateCommentReplyDto dto)
    {
        var comment = await _commentRepository.GetByIdAsync(dto.CommentId);
        if (comment == null)
            return ServiceResponse<CommentReplyDto>.Failure("Comentario no encontrado.");

        var reply = new Domain.Entities.Comment.CommentReply
        {
            CommentId = dto.CommentId,
            UserId = dto.UserId,
            Content = dto.Content.Trim()
        };

        await _replyRepository.AddAsync(reply);

        var user = await _userRepository.GetByIdAsync(dto.UserId);

        var result = new CommentReplyDto
        {
            Id = reply.Id,
            CommentId = reply.CommentId,
            UserId = reply.UserId,
            AuthorName = user != null ? $"{user.FirstName} {user.LastName}" : "Usuario",
            Content = reply.Content,
            CreatedAt = reply.CreatedAt
        };

        return ServiceResponse<CommentReplyDto>.Success(result);
    }

    public async Task<BaseResult> DeleteReplyAsync(Guid replyId, Guid userId)
    {
        var reply = await _replyRepository.GetByIdAsync(replyId);
        if (reply == null)
            return BaseResult.Fail("Respuesta no encontrada.");

        if (reply.UserId != userId)
            return BaseResult.Fail("No tienes permisos para eliminar esta respuesta.");

        await _replyRepository.DeleteAsync(reply);
        return BaseResult.Ok();
    }

    public async Task<List<CommentReplyDto>> GetRepliesByCommentAsync(Guid commentId)
    {
        var replies = await _replyRepository.Query()
            .Where(r => r.CommentId == commentId)
            .OrderBy(r => r.CreatedAt)
            .Include(r => r.User)
            .ToListAsync();

        return replies.Select(r => new CommentReplyDto
        {
            Id = r.Id,
            CommentId = r.CommentId,
            UserId = r.UserId,
            AuthorName = $"{r.User.FirstName} {r.User.LastName}",
            AuthorProfilePicture = r.User.ProfilePicturePath,
            Content = r.Content,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}
