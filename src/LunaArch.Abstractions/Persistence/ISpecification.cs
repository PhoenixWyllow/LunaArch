using System.Linq.Expressions;

namespace LunaArch.Abstractions.Persistence;

/// <summary>
/// Interface for the Specification pattern.
/// Specifications encapsulate query logic in a reusable, composable way.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the criteria expression for filtering entities.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the list of include expressions for eager loading related entities.
    /// </summary>
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the list of include strings for eager loading (for string-based includes).
    /// </summary>
    IReadOnlyList<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the order by expression for ascending sorting.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the order by expression for descending sorting.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the list of additional ordering expressions (ThenBy).
    /// </summary>
    IReadOnlyList<(Expression<Func<T, object>> Expression, bool IsDescending)> ThenByExpressions { get; }

    /// <summary>
    /// Gets the number of entities to skip.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the number of entities to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets a value indicating whether the query should be split into multiple queries.
    /// </summary>
    bool IsSplitQuery { get; }

    /// <summary>
    /// Gets a value indicating whether change tracking should be disabled.
    /// </summary>
    bool IsNoTracking { get; }

    /// <summary>
    /// Gets a value indicating whether to ignore query filters (e.g., soft delete filters).
    /// </summary>
    bool IgnoreQueryFilters { get; }
}
