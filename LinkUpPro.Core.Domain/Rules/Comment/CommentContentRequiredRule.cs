namespace LinkUpPro.Domain.Rules.Comment;

public class CommentContentRequiredRule(string content) : Common.IBusinessRule
{
    public string Message => "El comentario no puede estar vacío.";
    public bool IsBroken() => string.IsNullOrWhiteSpace(content);
}