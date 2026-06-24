namespace LinkUpPro.Domain.Rules.Battleship.Attack;
public class AttackerMustHaveActiveTurnRule(Guid currentTurnPlayerId, Guid attackerId) : Common.IBusinessRule
{
    public string Message => "No es tu turno de atacar.";
    public bool IsBroken() => currentTurnPlayerId != attackerId;
}