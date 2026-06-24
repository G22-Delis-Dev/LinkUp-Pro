namespace LinkUpPro.Domain.Rules.Post;
public class PostCannotHaveBothMediaRule(bool hasImages, bool hasVideos) : Common.IBusinessRule
{
    public string Message => "Un post no puede tener imágenes y videos al mismo tiempo.";
    public bool IsBroken() => hasImages && hasVideos;
}