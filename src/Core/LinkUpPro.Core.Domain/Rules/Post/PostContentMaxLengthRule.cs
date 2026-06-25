namespace LinkUpPro.Domain.Rules.Post;
public class PostContentMaxLengthRule(string content, int maxLength = 2000) : Common.IBusinessRule
{
    public string Message => $"El contenido del post no puede exceder los {maxLength} caracteres.";
    public bool IsBroken() => !string.IsNullOrEmpty(content) && content.Length > maxLength;
}