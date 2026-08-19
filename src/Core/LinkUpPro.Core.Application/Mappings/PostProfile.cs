using AutoMapper;
using LinkUpPro.Application.DTOs.Post;
using LinkUpPro.Application.ViewModels.Post;
using LinkUpPro.Domain.Entities.Post;

namespace LinkUpPro.Application.Mappings;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<CreatePostViewModel, CreatePostDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.ImageStream, opt => opt.MapFrom(src => src.Image != null ? src.Image.OpenReadStream() : null))
            .ForMember(dest => dest.ImageContentType, opt => opt.MapFrom(src => src.Image != null ? src.Image.ContentType : null))
            .ForMember(dest => dest.ImageFileName, opt => opt.MapFrom(src => src.Image != null ? src.Image.FileName : null));

        CreateMap<EditPostViewModel, UpdatePostDto>()
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<PostDto, PostViewModel>()
            .ForMember(dest => dest.TimeAgo, opt => opt.Ignore())
            .ForMember(dest => dest.IsOwner, opt => opt.Ignore());
    }
}
