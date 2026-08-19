using AutoMapper;
using LinkUpPro.Application.DTOs.Auth;
using LinkUpPro.Application.ViewModels.Auth;

namespace LinkUpPro.Application.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<LoginViewModel, LoginDto>();
        CreateMap<RegisterViewModel, RegisterDto>();
        CreateMap<ForgotPasswordViewModel, ForgotPasswordDto>();
        CreateMap<ResetPasswordViewModel, ResetPasswordDto>();
    }
}
