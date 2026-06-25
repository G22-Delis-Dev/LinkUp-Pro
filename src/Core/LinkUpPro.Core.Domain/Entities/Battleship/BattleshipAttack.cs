using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Battleship;
public class BattleshipAttack : AuditableEntity<Guid>
{
    public Guid BoardId { get; set; }
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public bool IsHit { get; set; }

    public BattleshipBoard Board { get; set; } = null!;
}