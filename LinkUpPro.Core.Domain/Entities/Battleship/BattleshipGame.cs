using LinkUpPro.Domain.Common;
using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Domain.Entities.Battleship;
public class BattleshipGame : AuditableEntity<Guid>
{
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
    public Guid? WinnerId { get; set; }
    public GameStatus Status { get; set; } = GameStatus.WaitingForOpponent;
    public GameResult Result { get; set; } = GameResult.None;
    public Guid CurrentTurnPlayerId { get; set; }

    public User.User Player1 { get; set; } = null!;
    public User.User Player2 { get; set; } = null!;
    public ICollection<BattleshipBoard> Boards { get; private set; } = new List<BattleshipBoard>();
}