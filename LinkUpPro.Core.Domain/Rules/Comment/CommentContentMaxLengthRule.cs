namespace LinkUpPro.Domain.Rules.Comment;

public class CommentContentMaxLengthRule(string content, int maxLength = 500) : Common.IBusinessRule
{
    public string Message => $"El comentario no puede exceder los {maxLength} caracteres.";
    public bool IsBroken() => content?.Length > maxLength;
}