namespace LunaArch.Domain.Rules;

/// <summary>
/// Interface for business rules that can be validated.
/// Use business rules to encapsulate complex domain validation logic.
/// </summary>
/// <example>
/// <code>
/// public sealed class OrderCannotExceedMaxItemsRule : IBusinessRule
/// {
///     private readonly int _currentItemCount;
///     private const int MaxItems = 100;
///
///     public OrderCannotExceedMaxItemsRule(int currentItemCount)
///     {
///         _currentItemCount = currentItemCount;
///     }
///
///     public bool IsBroken() => _currentItemCount >= MaxItems;
///
///     public string Message => $"Order cannot have more than {MaxItems} items.";
/// }
/// </code>
/// </example>
public interface IBusinessRule
{
    /// <summary>
    /// Determines whether this business rule is broken.
    /// </summary>
    /// <returns>True if the rule is violated; otherwise, false.</returns>
    bool IsBroken();

    /// <summary>
    /// Gets the error message to display when the rule is broken.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Gets the error code for this rule violation. Override to provide specific codes.
    /// </summary>
    string Code => GetType().Name;
}
