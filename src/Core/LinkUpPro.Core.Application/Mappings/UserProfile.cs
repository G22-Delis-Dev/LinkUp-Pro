using AutoMapper;
using LinkUpPro.Application.DTOs.User;
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
    }
}
