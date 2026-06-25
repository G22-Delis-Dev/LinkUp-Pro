namespace LinkUpPro.Domain.Rules.Battleship.Ship;
public class ShipCannotOverlapAnotherRule(bool overlaps) : Common.IBusinessRule
{
    public string Message => "El barco no puede superponerse con otro.";
    public bool IsBroken() => overlaps;
}