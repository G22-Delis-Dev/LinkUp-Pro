namespace LinkUpPro.Domain.Rules.User;
public class UserMustBeActiveRule(bool isActive) : Common.IBusinessRule
{
    public string Message => "El usuario debe estar activo para realizar esta acción.";
    public bool IsBroken() => !isActive;
}