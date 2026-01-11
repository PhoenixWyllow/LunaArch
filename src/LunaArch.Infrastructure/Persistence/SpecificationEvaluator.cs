using LunaArch.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LunaArch.Infrastructure.Persistence;

/// <summary>
/// Evaluates specifications and applies them to EF Core queries.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies a specification to a query.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="inputQuery">The input query.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="applyPaging">Whether to apply paging.</param>
    /// <returns>The query with the specification applied.</returns>
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> inputQuery,
        ISpecification<T> specification,
        bool applyPaging = true) where T : class
    {
        var query = inputQuery;

        // Apply ignore query filters
        if (specification.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        // Apply criteria
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        query = specification.Includes.Aggregate(
            query,
            (current, include) => current.Include(include));

        // Apply string-based includes
        query = specification.IncludeStrings.Aggregate(
            query,
            (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);

            // Apply ThenBy expressions
            var orderedQuery = (IOrderedQueryable<T>)query;
            foreach (var (expression, isDescending) in specification.ThenByExpressions)
            {
                orderedQuery = isDescending
                    ? orderedQuery.ThenByDescending(expression)
                    : orderedQuery.ThenBy(expression);
            }
            query = orderedQuery;
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);

            // Apply ThenBy expressions
            var orderedQuery = (IOrderedQueryable<T>)query;
            foreach (var (expression, isDescending) in specification.ThenByExpressions)
            {
                orderedQuery = isDescending
                    ? orderedQuery.ThenByDescending(expression)
                    : orderedQuery.ThenBy(expression);
            }
            query = orderedQuery;
        }

        // Apply paging
        if (applyPaging)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        // Apply split query
        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        // Apply tracking behavior
        if (specification.IsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
