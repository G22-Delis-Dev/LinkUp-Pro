namespace LinkUpPro.Domain.Rules.User;
public class PasswordStrengthRule(string password) : Common.IBusinessRule
{
    public string Message => "La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.";
    public bool IsBroken() => string.IsNullOrWhiteSpace(password) || password.Length < 8 || !password.Any(char.IsUpper) || !password.Any(char.IsDigit);
}