using AutoMapper;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Application.Mappings;

public class BattleshipProfile : Profile
{
    public BattleshipProfile()
    {
        CreateMap<BattleshipGame, BattleshipGameDto>()
            .ForMember(dest => dest.Player1Name, opt => opt.MapFrom(src => $"{src.Player1.FirstName} {src.Player1.LastName}"))
            .ForMember(dest => dest.Player2Name, opt => opt.MapFrom(src => $"{src.Player2.FirstName} {src.Player2.LastName}"));

        CreateMap<BattleshipBoard, BattleshipBoardDto>();
        CreateMap<BattleshipShip, ShipDto>()
            .ForMember(dest => dest.StartX, opt => opt.MapFrom(src => src.StartCoordinateX))
            .ForMember(dest => dest.StartY, opt => opt.MapFrom(src => src.StartCoordinateY));
            
        CreateMap<BattleshipAttack, AttackResultDto>();
    }
}
