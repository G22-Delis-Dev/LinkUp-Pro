using AutoMapper;
using LinkUpPro.Application.DTOs.Reaction;
using LinkUpPro.Domain.Entities.Reaction;

namespace LinkUpPro.Application.Mappings;

public class ReactionProfile : Profile
{
    public ReactionProfile()
    {
        CreateMap<Reaction, ReactionDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
    }
}
