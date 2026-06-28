using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;

namespace LinkUpPro.Application.Interfaces.Identity;

public interface IAccountActivationService
{
    Task<ServiceResponse<ActivationResultDto>> ActivateAsync(string userId, string token);
    Task<ServiceResponse<string>> ResendActivationAsync(string email);
}
