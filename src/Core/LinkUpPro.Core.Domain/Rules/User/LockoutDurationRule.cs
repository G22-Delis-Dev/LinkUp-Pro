namespace LinkUpPro.Domain.Rules.User;
public class LockoutDurationRule(DateTimeOffset? lockoutEnd) : Common.IBusinessRule
{
    public string Message => "La cuenta está bloqueada temporalmente.";
    public bool IsBroken() => lockoutEnd.HasValue && lockoutEnd.Value > DateTimeOffset.UtcNow;
}