namespace LinkUpPro.Application.DTOs.Battleship;

public class AttackResultDto
{
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public bool IsHit { get; set; }
    public bool IsSunk { get; set; }
    public bool IsGameOver { get; set; }
    public Guid? WinnerId { get; set; }
    public string? ShipSunkName { get; set; }
}
