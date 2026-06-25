namespace LinkUpPro.Domain.Rules.Reaction;

public class ReactionTypeMustBeValidRule(Enums.Reaction.ReactionType type) : Common.IBusinessRule
{
    public string Message => "El tipo de reacción no es válido.";
    public bool IsBroken() => !Enum.IsDefined(typeof(Enums.Reaction.ReactionType), type);
}