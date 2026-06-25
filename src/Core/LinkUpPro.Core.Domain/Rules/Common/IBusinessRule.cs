namespace LinkUpPro.Domain.Rules.Common;

public interface IBusinessRule
{
    string Message { get; }
    bool IsBroken();
}