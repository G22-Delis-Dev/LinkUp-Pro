using LinkUpPro.Application.Common;

namespace LinkUpPro.Application.Interfaces.Identity;

public interface ISessionService
{
    Task SignOutAsync();
    Task<BaseResult> InvalidateOtherSessionsAsync(Guid userId);
}
