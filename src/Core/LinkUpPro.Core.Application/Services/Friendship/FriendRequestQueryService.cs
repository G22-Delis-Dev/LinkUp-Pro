using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Application.Services.Friendship;

public class FriendRequestQueryService : IFriendRequestQueryService
{
    private readonly IFriendRequestRepository _requestRepository;

    public FriendRequestQueryService(IFriendRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<List<FriendRequestDto>> GetReceivedRequestsAsync(Guid userId)
    {
        var requests = await _requestRepository.Query()
            .Where(r => r.ReceiverId == userId && r.Status == FriendRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Sender)
            .ToListAsync();

        return requests.Select(r => new FriendRequestDto
        {
            Id = r.Id,
            SenderId = r.SenderId,
            SenderName = $"{r.Sender.FirstName} {r.Sender.LastName}",
            SenderProfilePicture = r.Sender.ProfilePicturePath,
            ReceiverId = r.ReceiverId,
            Status = r.Status,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<List<FriendRequestDto>> GetSentRequestsAsync(Guid userId)
    {
        var requests = await _requestRepository.Query()
            .Where(r => r.SenderId == userId && r.Status == FriendRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .Include(r => r.Receiver)
            .ToListAsync();

        return requests.Select(r => new FriendRequestDto
        {
            Id = r.Id,
            SenderId = r.SenderId,
            SenderName = "Me",
            ReceiverId = r.ReceiverId,
            Status = r.Status,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<bool> HasPendingRequestAsync(Guid senderId, Guid receiverId)
    {
        return await _requestRepository.ExistsAsync(r =>
            ((r.SenderId == senderId && r.ReceiverId == receiverId) ||
             (r.SenderId == receiverId && r.ReceiverId == senderId)) &&
            r.Status == FriendRequestStatus.Pending);
    }
}
