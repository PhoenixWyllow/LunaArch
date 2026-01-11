namespace LunaArch.Domain.Rules;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public sealed class BusinessRuleValidationException : Exception
{
    /// <summary>
    /// Gets the business rule that was broken.
    /// </summary>
    public IBusinessRule BrokenRule { get; }

    /// <summary>
    /// Gets the error code associated with the broken rule.
    /// </summary>
    public string Code => BrokenRule.Code;

    /// <summary>
    /// Gets additional details about the rule violation.
    /// </summary>
    public string Details { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleValidationException"/> class.
    /// </summary>
    /// <param name="brokenRule">The business rule that was broken.</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    /// <summary>
    /// Initializes a new instance with additional details.
    /// </summary>
    /// <param name="brokenRule">The business rule that was broken.</param>
    /// <param name="details">Additional details about the violation.</param>
    public BusinessRuleValidationException(IBusinessRule brokenRule, string details)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = details;
    }
}
