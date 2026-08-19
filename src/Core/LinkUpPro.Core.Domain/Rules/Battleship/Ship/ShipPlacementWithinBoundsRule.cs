namespace LinkUpPro.Domain.Rules.Battleship.Ship;

public class ShipPlacementWithinBoundsRule(int startX, int startY, int size, Enums.Battleship.ShipDirection direction, int boardSize = 12) : Common.IBusinessRule
{
    public string Message => "El barco se sale de los límites del tablero.";
    
    public bool IsBroken()
    {
        // Validar que la celda inicial esté dentro del tablero
        if (startX < 0 || startX >= boardSize || startY < 0 || startY >= boardSize)
            return true;

        // Validar según la dirección
        return direction switch
        {
            Enums.Battleship.ShipDirection.Right => startX + size > boardSize,
            Enums.Battleship.ShipDirection.Left => startX - size + 1 < 0,
            Enums.Battleship.ShipDirection.Down => startY + size > boardSize,
            Enums.Battleship.ShipDirection.Up => startY - size + 1 < 0,
            _ => true
        };
    }
}