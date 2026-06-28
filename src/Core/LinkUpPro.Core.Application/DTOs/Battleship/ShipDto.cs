using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.DTOs.Battleship;

public class ShipDto
{
    public Guid Id { get; set; }
    public ShipSize Size { get; set; }
    public ShipDirection Direction { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
    public bool IsSunk { get; set; }
}
