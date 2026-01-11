using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Persistence;
using LunaArch.Abstractions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace LunaArch.Infrastructure.Persistence;

/// <summary>
/// Base implementation of the read-only repository pattern using Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The identifier type.</typeparam>
/// <remarks>
/// EF Core is not fully AOT/trimming compatible. See https://aka.ms/efcore-docs-trimming
/// </remarks>
[SuppressMessage("Trimming", "IL2091:Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method", Justification = "EF Core is not fully AOT compatible")]
public class ReadRepositoryBase<TEntity, TId>(DbContext context) : IReadRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets the DbContext instance.
    /// </summary>
    protected DbContext Context { get; } = context;

    /// <summary>
    /// Gets the DbSet for the entity.
    /// </summary>
    protected DbSet<TEntity> DbSet => Context.Set<TEntity>();

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification, applyPaging: false);
        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification, applyPaging: false);
        return await query.AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task<PagedResult<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification, applyPaging: false);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TEntity>(items, pageNumber, pageSize, totalCount);
    }

    /// <summary>
    /// Applies a specification to the query.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="applyPaging">Whether to apply paging from the specification.</param>
    /// <returns>The query with the specification applied.</returns>
    protected virtual IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> specification,
        bool applyPaging = true)
    {
        return SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification, applyPaging);
    }
}
