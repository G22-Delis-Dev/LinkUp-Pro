using AutoMapper;
using LinkUpPro.Application.DTOs.Comment;
using LinkUpPro.Domain.Entities.Comment;

namespace LinkUpPro.Application.Mappings;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.AuthorProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicturePath))
            .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies.Count));

        CreateMap<CommentReply, CommentReplyDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.AuthorProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicturePath));
    }
}
