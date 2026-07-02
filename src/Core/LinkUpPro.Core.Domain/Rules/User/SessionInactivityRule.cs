namespace LinkUpPro.Domain.Rules.User;
public class SessionInactivityRule(DateTime lastActivity, TimeSpan maxInactivity) : Common.IBusinessRule
{
    public string Message => "La sesión ha expirado por inactividad.";
    public bool IsBroken() => DateTime.UtcNow - lastActivity > maxInactivity;
}
