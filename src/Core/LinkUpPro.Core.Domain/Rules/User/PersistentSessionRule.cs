namespace LinkUpPro.Domain.Rules.User;

public class PersistentSessionRule(bool isPersistent, DateTime expiration) : Common.IBusinessRule
{
    public string Message => "La sesión persistente ha expirado.";
    public bool IsBroken() => isPersistent && DateTime.UtcNow > expiration;
}