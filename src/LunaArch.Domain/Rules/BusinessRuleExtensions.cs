namespace LunaArch.Domain.Rules;

/// <summary>
/// Extension methods for checking business rules.
/// </summary>
public static class BusinessRuleExtensions
{
    /// <summary>
    /// Checks a business rule and throws if it is broken.
    /// </summary>
    /// <param name="rule">The rule to check.</param>
    /// <exception cref="BusinessRuleValidationException">Thrown when the rule is broken.</exception>
    public static void CheckRule(this IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    /// <summary>
    /// Checks multiple business rules and throws if any are broken.
    /// </summary>
    /// <param name="rules">The rules to check.</param>
    /// <exception cref="BusinessRuleValidationException">Thrown when any rule is broken.</exception>
    public static void CheckRules(this IEnumerable<IBusinessRule> rules)
    {
        foreach (var rule in rules)
        {
            rule.CheckRule();
        }
    }
}
