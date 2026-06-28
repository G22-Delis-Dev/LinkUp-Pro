using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.Notification;
using LinkUpPro.Domain.Enums.Friendship;
using LinkUpPro.Domain.Enums.Notification;
using LinkUpPro.Domain.Interfaces.Repositories.Friendship;
using LinkUpPro.Domain.Interfaces.Repositories.User;

namespace LinkUpPro.Application.Services.Friendship;

public class FriendRequestService : IFriendRequestService
{
    private readonly IFriendRequestRepository _requestRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationDispatchService _notificationDispatch;

    public FriendRequestService(
        IFriendRequestRepository requestRepository,
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        INotificationDispatchService notificationDispatch)
    {
        _requestRepository = requestRepository;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _notificationDispatch = notificationDispatch;
    }

    public async Task<ServiceResponse<FriendRequestDto>> SendRequestAsync(SendFriendRequestDto dto)
    {
        if (dto.SenderId == dto.ReceiverId)
            return ServiceResponse<FriendRequestDto>.Failure("No puedes enviarte solicitud a ti mismo.");

        var existingRequest = await _requestRepository.FindOneAsync(r =>
            (r.SenderId == dto.SenderId && r.ReceiverId == dto.ReceiverId && r.Status == FriendRequestStatus.Pending) ||
            (r.SenderId == dto.ReceiverId && r.ReceiverId == dto.SenderId && r.Status == FriendRequestStatus.Pending));

        if (existingRequest != null)
            return ServiceResponse<FriendRequestDto>.Failure("Ya existe una solicitud pendiente.");

        var areFriends = await _friendshipRepository.ExistsAsync(f =>
            ((f.UserId == dto.SenderId && f.FriendId == dto.ReceiverId) ||
             (f.UserId == dto.ReceiverId && f.FriendId == dto.SenderId)) &&
            f.Status == FriendshipStatus.Active);

        if (areFriends)
            return ServiceResponse<FriendRequestDto>.Failure("Ya son amigos.");

        var request = new Domain.Entities.Friendship.FriendRequest
        {
            SenderId = dto.SenderId,
            ReceiverId = dto.ReceiverId,
            Status = FriendRequestStatus.Pending
        };

        await _requestRepository.AddAsync(request);

        var sender = await _userRepository.GetByIdAsync(dto.SenderId);
        
        await _notificationDispatch.SendNotificationAsync(
            dto.ReceiverId,
            NotificationType.FriendRequestReceived,
            $"{sender?.FirstName} {sender?.LastName} te ha enviado una solicitud de amistad.",
            request.Id.ToString());

        var result = new FriendRequestDto
        {
            Id = request.Id,
            SenderId = request.SenderId,
            SenderName = $"{sender?.FirstName} {sender?.LastName}",
            ReceiverId = request.ReceiverId,
            Status = request.Status,
            CreatedAt = request.CreatedAt
        };

        return ServiceResponse<FriendRequestDto>.Success(result);
    }

    public async Task<BaseResult> AcceptRequestAsync(Guid requestId, Guid userId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null) return BaseResult.Fail("Solicitud no encontrada.");
        if (request.ReceiverId != userId) return BaseResult.Fail("No autorizado.");
        if (request.Status != FriendRequestStatus.Pending) return BaseResult.Fail("Solicitud ya procesada.");

        request.Status = FriendRequestStatus.Accepted;
        await _requestRepository.UpdateAsync(request);

        // Crear la amistad
        var friendship = new Domain.Entities.Friendship.Friendship
        {
            // Siempre guardar el menor Id primero para evitar duplicados, 
            // aunque EntityFramework index manejará esto.
            UserId = request.SenderId,
            FriendId = request.ReceiverId,
            Status = FriendshipStatus.Active
        };

        await _friendshipRepository.AddAsync(friendship);

        var receiver = await _userRepository.GetByIdAsync(userId);

        await _notificationDispatch.SendNotificationAsync(
            request.SenderId,
            NotificationType.FriendRequestAccepted,
            $"{receiver?.FirstName} {receiver?.LastName} ha aceptado tu solicitud de amistad.",
            friendship.Id.ToString());

        return BaseResult.Ok();
    }

    public async Task<BaseResult> RejectRequestAsync(Guid requestId, Guid userId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null) return BaseResult.Fail("Solicitud no encontrada.");
        if (request.ReceiverId != userId) return BaseResult.Fail("No autorizado.");

        request.Status = FriendRequestStatus.Rejected;
        await _requestRepository.UpdateAsync(request);

        return BaseResult.Ok();
    }

    public async Task<BaseResult> CancelRequestAsync(Guid requestId, Guid userId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null) return BaseResult.Fail("Solicitud no encontrada.");
        if (request.SenderId != userId) return BaseResult.Fail("No autorizado.");

        request.Status = FriendRequestStatus.Canceled;
        await _requestRepository.UpdateAsync(request);

        return BaseResult.Ok();
    }
}
