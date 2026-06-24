namespace LinkUpPro.Domain.Rules.Post;
public class PostPrivacyMustBeValidRule(Enums.Post.PostPrivacy privacy) : Common.IBusinessRule
{
    public string Message => "La configuración de privacidad no es válida.";
    public bool IsBroken() => !Enum.IsDefined(typeof(Enums.Post.PostPrivacy), privacy);
}