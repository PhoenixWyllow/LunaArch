using System.Linq.Expressions;
using LunaArch.Abstractions.Persistence;

namespace LunaArch.Domain.Specifications;

/// <summary>
/// Base class for implementing the Specification pattern.
/// Provides a fluent API for building query specifications.
/// </summary>
/// <typeparam name="T">The type of entity this specification applies to.</typeparam>
/// <example>
/// <code>
/// public sealed class ActiveOrdersSpec : Specification&lt;Order&gt;
/// {
///     public ActiveOrdersSpec(Guid customerId)
///     {
///         AddCriteria(o => o.CustomerId == customerId &amp;&amp; !o.IsCompleted);
///         AddInclude(o => o.Items);
///         ApplyOrderBy(o => o.CreatedAt);
///     }
/// }
/// </code>
/// </example>
public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<(Expression<Func<T, object>> Expression, bool IsDescending)> _thenByExpressions = [];

    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<(Expression<Func<T, object>> Expression, bool IsDescending)> ThenByExpressions =>
        _thenByExpressions.AsReadOnly();

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <inheritdoc />
    public bool IsSplitQuery { get; private set; }

    /// <inheritdoc />
    public bool IsNoTracking { get; private set; } = true;

    /// <inheritdoc />
    public bool IgnoreQueryFilters { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class.
    /// </summary>
    protected Specification()
    {
    }

    /// <summary>
    /// Initializes a new instance with a criteria expression.
    /// </summary>
    /// <param name="criteria">The filter criteria.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds a filter criteria to the specification.
    /// </summary>
    /// <param name="criteria">The criteria expression.</param>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds an include expression for eager loading.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        _includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds a string-based include for eager loading (supports nested includes like "Items.Product").
    /// </summary>
    /// <param name="includeString">The include string.</param>
    protected void AddInclude(string includeString)
    {
        _includeStrings.Add(includeString);
    }

    /// <summary>
    /// Applies ascending ordering.
    /// </summary>
    /// <param name="orderByExpression">The ordering expression.</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Applies descending ordering.
    /// </summary>
    /// <param name="orderByDescendingExpression">The ordering expression.</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Adds a secondary ascending ordering.
    /// </summary>
    /// <param name="thenByExpression">The secondary ordering expression.</param>
    protected void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
    {
        _thenByExpressions.Add((thenByExpression, false));
    }

    /// <summary>
    /// Adds a secondary descending ordering.
    /// </summary>
    /// <param name="thenByDescendingExpression">The secondary ordering expression.</param>
    protected void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
    {
        _thenByExpressions.Add((thenByDescendingExpression, true));
    }

    /// <summary>
    /// Applies pagination to the specification.
    /// </summary>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    /// <summary>
    /// Enables split queries for better performance with multiple includes.
    /// </summary>
    protected void ApplySplitQuery()
    {
        IsSplitQuery = true;
    }

    /// <summary>
    /// Enables change tracking for this query.
    /// </summary>
    protected void ApplyTracking()
    {
        IsNoTracking = false;
    }

    /// <summary>
    /// Ignores global query filters (e.g., soft delete filters).
    /// </summary>
    protected void ApplyIgnoreQueryFilters()
    {
        IgnoreQueryFilters = true;
    }
}
