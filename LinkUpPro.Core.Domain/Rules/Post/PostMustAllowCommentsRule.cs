namespace LinkUpPro.Domain.Rules.Post;
public class PostMustAllowCommentsRule(bool allowComments) : Common.IBusinessRule
{
    public string Message => "Este post no permite comentarios.";
    public bool IsBroken() => !allowComments;
}