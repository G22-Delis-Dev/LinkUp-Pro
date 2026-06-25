using LinkUpPro.Domain.Common;

namespace LinkUpPro.Domain.Entities.Battleship;
public class BattleshipBoard : BaseEntity<Guid>
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }

    public BattleshipGame Game { get; set; } = null!;
    public User.User Player { get; set; } = null!;
    public ICollection<BattleshipShip> Ships { get; private set; } = new List<BattleshipShip>();
    public ICollection<BattleshipAttack> ReceivedAttacks { get; private set; } = new List<BattleshipAttack>();
}