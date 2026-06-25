namespace LinkUpPro.Domain.Rules.Post;
public class PostContentRequiredRule(string content, bool hasMedia) : Common.IBusinessRule
{
    public string Message => "El post debe tener contenido de texto o contenido multimedia.";
    public bool IsBroken() => string.IsNullOrWhiteSpace(content) && !hasMedia;
}