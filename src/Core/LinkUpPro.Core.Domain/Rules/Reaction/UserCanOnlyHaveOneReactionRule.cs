namespace LinkUpPro.Domain.Rules.Reaction;

public class UserCanOnlyHaveOneReactionRule(bool hasExistingReaction) : Common.IBusinessRule
{
    public string Message => "El usuario ya ha reaccionado a este contenido.";
    public bool IsBroken() => hasExistingReaction;
}