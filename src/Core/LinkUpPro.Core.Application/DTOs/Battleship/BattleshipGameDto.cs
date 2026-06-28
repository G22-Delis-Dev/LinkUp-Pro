using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.DTOs.Battleship;

public class BattleshipGameDto
{
    public Guid Id { get; set; }
    public Guid Player1Id { get; set; }
    public string Player1Name { get; set; } = null!;
    public Guid Player2Id { get; set; }
    public string Player2Name { get; set; } = null!;
    public GameStatus Status { get; set; }
    public GameResult Result { get; set; }
    public Guid CurrentTurnPlayerId { get; set; }
    public Guid? WinnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}
