namespace LinkUpPro.Domain.Rules.Post;
public class PostMustHaveMediaRule(bool hasMedia, Enums.Post.PostContentType type) : Common.IBusinessRule
{
    public string Message => "El tipo de post requiere contenido multimedia.";
    public bool IsBroken() => (type == Enums.Post.PostContentType.Image || type == Enums.Post.PostContentType.Video) && !hasMedia;
}