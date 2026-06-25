using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Domain.Entities.Battleship;
public class BattleshipShip : BaseEntity<Guid>
{
    public Guid BoardId { get; set; }
    public ShipSize Size { get; set; }
    public ShipDirection Direction { get; set; }
    public int StartCoordinateX { get; set; }
    public int StartCoordinateY { get; set; }
    public bool IsSunk { get; set; }

    public BattleshipBoard Board { get; set; } = null!;
}