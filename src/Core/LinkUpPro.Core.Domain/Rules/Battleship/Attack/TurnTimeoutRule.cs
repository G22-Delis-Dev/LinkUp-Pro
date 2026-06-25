namespace LinkUpPro.Domain.Rules.Battleship.Attack;
public class TurnTimeoutRule(DateTime turnStartTime, int maxSeconds = 60) : Common.IBusinessRule
{
    public string Message => "El tiempo del turno ha expirado.";
    public bool IsBroken() => (DateTime.UtcNow - turnStartTime).TotalSeconds > maxSeconds;
}