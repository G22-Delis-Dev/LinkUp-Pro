using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Friendship;

namespace LinkUpPro.Application.Interfaces.Friendship;

public interface IFriendRequestService
{
    Task<ServiceResponse<FriendRequestDto>> SendRequestAsync(SendFriendRequestDto dto);
    Task<BaseResult> AcceptRequestAsync(Guid requestId, Guid userId);
    Task<BaseResult> RejectRequestAsync(Guid requestId, Guid userId);
    Task<BaseResult> CancelRequestAsync(Guid requestId, Guid userId);
    Task<BaseResult> HideRequestAsync(Guid requestId, Guid userId);
}
