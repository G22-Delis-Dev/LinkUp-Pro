using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;

namespace LinkUpPro.Application.Interfaces.Identity;

public interface ILoginService
{
    Task<ServiceResponse<string>> LoginAsync(LoginDto dto);
}
