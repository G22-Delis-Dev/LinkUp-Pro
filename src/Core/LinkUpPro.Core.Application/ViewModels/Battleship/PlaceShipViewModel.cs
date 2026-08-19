using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.ViewModels.Battleship;

public class PlaceShipViewModel
{
    public Guid GameId { get; set; }
    public ShipSize Size { get; set; }
    public ShipDirection Direction { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
}
