using AutoMapper;
using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Domain.Entities.Friendship;

namespace LinkUpPro.Application.Mappings;

public class FriendshipProfile : Profile
{
    public FriendshipProfile()
    {
        // En DTOs las relaciones se resuelven a mano mayormente (por temas de orden de Ids)
        // Pero registramos para posibles mapeos directos.
        CreateMap<Friendship, FriendshipDto>();
        CreateMap<FriendRequest, FriendRequestDto>();
    }
}
