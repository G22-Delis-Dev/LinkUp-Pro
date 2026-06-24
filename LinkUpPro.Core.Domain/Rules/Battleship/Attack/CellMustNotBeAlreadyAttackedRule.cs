namespace LinkUpPro.Domain.Rules.Battleship.Attack;
public class CellMustNotBeAlreadyAttackedRule(bool isAlreadyAttacked) : Common.IBusinessRule
{
    public string Message => "Esta coordenada ya ha sido atacada.";
    public bool IsBroken() => isAlreadyAttacked;
}