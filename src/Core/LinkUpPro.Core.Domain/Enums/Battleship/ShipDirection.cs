namespace LinkUpPro.Domain.Enums.Battleship;

/// <summary>
/// Dirección del barco desde su celda inicial.
/// Up: hacia arriba (Y decrece)
/// Down: hacia abajo (Y crece)  
/// Left: hacia izquierda (X decrece)
/// Right: hacia derecha (X crece)
/// </summary>
public enum ShipDirection 
{ 
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}