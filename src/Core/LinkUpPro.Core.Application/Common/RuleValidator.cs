using LinkUpPro.Domain.Exceptions;
using LinkUpPro.Domain.Rules.Common;

namespace LinkUpPro.Application.Common;

// Helper para validar reglas de negocio del dominio (IBusinessRule)
public static class RuleValidator
{
    public static void Validate(params IBusinessRule[] rules)
    {
        foreach (var rule in rules)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleViolationException(rule.Message);
            }
        }
    }

    public static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleViolationException(rule.Message);
        }
    }
}
