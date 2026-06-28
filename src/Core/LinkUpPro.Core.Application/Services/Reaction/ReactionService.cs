using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Reaction;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Application.Interfaces.Reaction;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Enums.Reaction;
using LinkUpPro.Domain.Interfaces.Repositories.Post;
using LinkUpPro.Domain.Interfaces.Repositories.Reaction;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Reaction;

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationDispatchService _notificationDispatch;

    public ReactionService(
        IReactionRepository reactionRepository,
        IPostRepository postRepository,
        IUserRepository userRepository,
        INotificationDispatchService notificationDispatch)
    {
        _reactionRepository = reactionRepository;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _notificationDispatch = notificationDispatch;
    }

    public async Task<ServiceResponse<ReactionDto>> ToggleReactionAsync(ToggleReactionDto dto)
    {
        var post = await _postRepository.GetByIdAsync(dto.PostId);
        if (post == null)
            return ServiceResponse<ReactionDto>.Failure("Publicación no encontrada.");

        var existingReaction = await _reactionRepository.FindOneAsync(r => r.PostId == dto.PostId && r.UserId == dto.UserId);

        var user = await _userRepository.GetByIdAsync(dto.UserId);
        var authorName = user != null ? $"{user.FirstName} {user.LastName}" : "Usuario";

        if (existingReaction != null)
        {
            if (existingReaction.Type == dto.Type)
            {
                // Misma reacción = Quitar
                await _reactionRepository.DeleteAsync(existingReaction);
                return ServiceResponse<ReactionDto>.Success(new ReactionDto { Id = existingReaction.Id, PostId = dto.PostId, UserId = dto.UserId, AuthorName = authorName, Type = dto.Type });
            }
            else
            {
                // Distinta reacción = Cambiar
                existingReaction.Type = dto.Type;
                await _reactionRepository.UpdateAsync(existingReaction);
                return ServiceResponse<ReactionDto>.Success(new ReactionDto { Id = existingReaction.Id, PostId = dto.PostId, UserId = dto.UserId, AuthorName = authorName, Type = dto.Type });
            }
        }

        // Nueva reacción = Crear
        var newReaction = new Domain.Entities.Reaction.Reaction
        {
            PostId = dto.PostId,
            UserId = dto.UserId,
            Type = dto.Type
        };

        await _reactionRepository.AddAsync(newReaction);

        // Notificar al autor
        if (post.UserId != dto.UserId)
        {
            await _notificationDispatch.SendNotificationAsync(
                post.UserId,
                NotificationType.NewReaction,
                $"{authorName} reaccionó a tu publicación.",
                newReaction.Id.ToString());
        }

        return ServiceResponse<ReactionDto>.Success(new ReactionDto { Id = newReaction.Id, PostId = dto.PostId, UserId = dto.UserId, AuthorName = authorName, Type = dto.Type });
    }

    public async Task<List<ReactionDto>> GetReactionsByPostAsync(Guid postId)
    {
        var reactions = await _reactionRepository.Query()
            .Where(r => r.PostId == postId)
            .Include(r => r.User)
            .ToListAsync();

        return reactions.Select(r => new ReactionDto
        {
            Id = r.Id,
            PostId = r.PostId,
            UserId = r.UserId,
            AuthorName = $"{r.User.FirstName} {r.User.LastName}",
            Type = r.Type
        }).ToList();
    }

    public async Task<Dictionary<ReactionType, int>> GetReactionCountsAsync(Guid postId)
    {
        var reactions = await _reactionRepository.Query()
            .Where(r => r.PostId == postId)
            .GroupBy(r => r.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return reactions.ToDictionary(r => r.Type, r => r.Count);
    }
}
