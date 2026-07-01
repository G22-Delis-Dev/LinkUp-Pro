using AutoMapper;
using LinkUpPro.Application.DTOs.User;
using LinkUpPro.Application.ViewModels.User;
using LinkUpPro.Domain.Entities.User;

namespace LinkUpPro.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore());

        CreateMap<UserProfileDto, UpdateProfileDto>();

        // ViewModel → DTO para edición de perfil
        CreateMap<UpdateProfileViewModel, UpdateProfileDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}
