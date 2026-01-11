using System.Linq.Expressions;
using LunaArch.Abstractions.Primitives;

namespace LunaArch.Abstractions.Persistence;

/// <summary>
/// Interface for read-only repository operations.
/// Use this interface when you only need to query data without modifications.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public interface IReadRepository<TEntity, in TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all entities.</returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of matching entities.</returns>
    Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the first entity matching the specification, or null if none found.
    /// </summary>
    /// <param name="specification">The specification to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The first matching entity, or null.</returns>
    Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter by.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The count of matching entities.</returns>
    Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all entities.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The total count of entities.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any entities match the specified specification.
    /// </summary>
    /// <param name="specification">The specification to check.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if any matching entities exist; otherwise, false.</returns>
    Task<bool> ExistsAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paged list of entities matching the specification.
    /// </summary>
    /// <param name="specification">The specification to filter by.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A paged result of entities.</returns>
    Task<PagedResult<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
