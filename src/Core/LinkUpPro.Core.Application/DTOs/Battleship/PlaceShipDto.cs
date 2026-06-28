using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.DTOs.Battleship;

public class PlaceShipDto
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
    public ShipSize Size { get; set; }
    public ShipDirection Direction { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
}
