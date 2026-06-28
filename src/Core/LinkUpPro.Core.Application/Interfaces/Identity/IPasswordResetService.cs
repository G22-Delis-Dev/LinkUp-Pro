using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;

namespace LinkUpPro.Application.Interfaces.Identity;

public interface IPasswordResetService
{
    Task<ServiceResponse<string>> RequestResetAsync(ForgotPasswordDto dto);
    Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto dto);
}
