using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;

namespace LinkUpPro.Application.Interfaces.Identity;

public interface IRegisterService
{
    Task<ServiceResponse<string>> RegisterAsync(RegisterDto dto);
}
