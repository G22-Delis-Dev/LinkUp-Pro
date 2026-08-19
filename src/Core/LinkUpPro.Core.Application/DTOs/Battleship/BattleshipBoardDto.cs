namespace LinkUpPro.Application.DTOs.Battleship;

public class BattleshipBoardDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
    public List<ShipDto> Ships { get; set; } = new();
    public List<AttackResultDto> ReceivedAttacks { get; set; } = new();
}
