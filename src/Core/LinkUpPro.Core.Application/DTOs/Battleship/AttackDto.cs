namespace LinkUpPro.Application.DTOs.Battleship;

public class AttackDto
{
    public Guid GameId { get; set; }
    public Guid AttackerPlayerId { get; set; }
    public int TargetX { get; set; }
    public int TargetY { get; set; }
}
