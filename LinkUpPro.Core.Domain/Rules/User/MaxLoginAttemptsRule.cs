namespace LinkUpPro.Domain.Rules.User;
public class MaxLoginAttemptsRule(int attempts) : Common.IBusinessRule
{
    public string Message => "Se ha excedido el número máximo de intentos de inicio de sesión.";
    public bool IsBroken() => attempts >= 5;
}