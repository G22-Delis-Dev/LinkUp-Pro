namespace LinkUpPro.Domain.Rules.Battleship.Board;

public class AllShipsMustBePlacedRule(int currentShips, int requiredShips = 5) : Common.IBusinessRule
{
    public string Message => "Debes posicionar todos tus barcos antes de empezar.";
    public bool IsBroken() => currentShips < requiredShips;
}