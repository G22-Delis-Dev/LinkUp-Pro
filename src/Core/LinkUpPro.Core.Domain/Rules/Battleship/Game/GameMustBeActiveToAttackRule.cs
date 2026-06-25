namespace LinkUpPro.Domain.Rules.Battleship.Game;
public class GameMustBeActiveToAttackRule(Enums.Battleship.GameStatus status) : Common.IBusinessRule
{
    public string Message => "El juego debe estar en progreso para poder atacar.";
    public bool IsBroken() => status != Enums.Battleship.GameStatus.InProgress;
}