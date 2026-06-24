namespace LinkUpPro.Domain.Rules.Battleship.Ship;
public class ShipPlacementWithinBoundsRule(int startX, int startY, int size, Enums.Battleship.ShipDirection direction, int boardSize = 10) : Common.IBusinessRule
{
    public string Message => "El barco se sale de los límites del tablero.";
    public bool IsBroken() =>
        startX < 0 || startY < 0 ||
        (direction == Enums.Battleship.ShipDirection.Horizontal && startX + size > boardSize) ||
        (direction == Enums.Battleship.ShipDirection.Vertical && startY + size > boardSize);
}