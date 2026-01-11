using System.Linq.Expressions;
using LunaArch.Abstractions.Persistence;

namespace LunaArch.Domain.Specifications;

/// <summary>
/// Extension methods for combining specifications.
/// </summary>
public static class SpecificationExtensions
{
    extension<T>(ISpecification<T> left)
    {
        /// <summary>
        /// Combines two specifications with a logical AND.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="left">The left specification.</param>
        /// <param name="right">The right specification.</param>
        /// <returns>A new specification representing the AND combination.</returns>
        public ISpecification<T> And(ISpecification<T> right)
        {
            return new AndSpecification<T>(left, right);
        }

        /// <summary>
        /// Combines two specifications with a logical OR.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="left">The left specification.</param>
        /// <param name="right">The right specification.</param>
        /// <returns>A new specification representing the OR combination.</returns>
        public ISpecification<T> Or(ISpecification<T> right)
        {
            return new OrSpecification<T>(left, right);
        }
    }

    extension<T>(ISpecification<T> specification)
    {
        /// <summary>
        /// Negates a specification.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="specification">The specification to negate.</param>
        /// <returns>A new specification representing the negation.</returns>
        public ISpecification<T> Not()
        {
            return new NotSpecification<T>(specification);
        }
    }
}

/// <summary>
/// Specification that combines two specifications with AND.
/// </summary>
internal sealed class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right) : CompositeSpecification<T>(left, right)
{
    public override Expression<Func<T, bool>>? Criteria
    {
        get
        {
            var leftCriteria = Left.Criteria;
            var rightCriteria = Right.Criteria;

            if (leftCriteria is null)
            {
                return rightCriteria;
            }

            if (rightCriteria is null)
            {
                return leftCriteria;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var combined = Expression.AndAlso(
                Expression.Invoke(leftCriteria, parameter),
                Expression.Invoke(rightCriteria, parameter));

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}

/// <summary>
/// Specification that combines two specifications with OR.
/// </summary>
internal sealed class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right) : CompositeSpecification<T>(left, right)
{
    public override Expression<Func<T, bool>>? Criteria
    {
        get
        {
            var leftCriteria = Left.Criteria;
            var rightCriteria = Right.Criteria;

            if (leftCriteria is null || rightCriteria is null)
            {
                return leftCriteria ?? rightCriteria;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var combined = Expression.OrElse(
                Expression.Invoke(leftCriteria, parameter),
                Expression.Invoke(rightCriteria, parameter));

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}

/// <summary>
/// Specification that negates another specification.
/// </summary>
internal sealed class NotSpecification<T>(ISpecification<T> specification) : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria
    {
        get
        {
            var innerCriteria = specification.Criteria;
            if (innerCriteria is null)
            {
                return null;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            var negated = Expression.Not(Expression.Invoke(innerCriteria, parameter));

            return Expression.Lambda<Func<T, bool>>(negated, parameter);
        }
    }

    public IReadOnlyList<Expression<Func<T, object>>> Includes => specification.Includes;
    public IReadOnlyList<string> IncludeStrings => specification.IncludeStrings;
    public Expression<Func<T, object>>? OrderBy => specification.OrderBy;
    public Expression<Func<T, object>>? OrderByDescending => specification.OrderByDescending;
    public IReadOnlyList<(Expression<Func<T, object>> Expression, bool IsDescending)> ThenByExpressions =>
        specification.ThenByExpressions;
    public int? Skip => specification.Skip;
    public int? Take => specification.Take;
    public bool IsSplitQuery => specification.IsSplitQuery;
    public bool IsNoTracking => specification.IsNoTracking;
    public bool IgnoreQueryFilters => specification.IgnoreQueryFilters;
}

/// <summary>
/// Base class for composite specifications.
/// </summary>
internal abstract class CompositeSpecification<T>(ISpecification<T> left, ISpecification<T> right) : ISpecification<T>
{
    protected ISpecification<T> Left { get; } = left;
    protected ISpecification<T> Right { get; } = right;

    public abstract Expression<Func<T, bool>>? Criteria { get; }

    public IReadOnlyList<Expression<Func<T, object>>> Includes =>
        Left.Includes.Concat(Right.Includes).Distinct().ToList();

    public IReadOnlyList<string> IncludeStrings =>
        Left.IncludeStrings.Concat(Right.IncludeStrings).Distinct().ToList();

    public Expression<Func<T, object>>? OrderBy => Left.OrderBy ?? Right.OrderBy;
    public Expression<Func<T, object>>? OrderByDescending => Left.OrderByDescending ?? Right.OrderByDescending;

    public IReadOnlyList<(Expression<Func<T, object>> Expression, bool IsDescending)> ThenByExpressions =>
        Left.ThenByExpressions.Concat(Right.ThenByExpressions).ToList();

    public int? Skip => Left.Skip ?? Right.Skip;
    public int? Take => Left.Take ?? Right.Take;
    public bool IsSplitQuery => Left.IsSplitQuery || Right.IsSplitQuery;
    public bool IsNoTracking => Left.IsNoTracking && Right.IsNoTracking;
    public bool IgnoreQueryFilters => Left.IgnoreQueryFilters || Right.IgnoreQueryFilters;
}
