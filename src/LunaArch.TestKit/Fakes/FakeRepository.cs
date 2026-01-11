using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Persistence;
using LunaArch.Abstractions.Primitives;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="IRepository{TEntity, TId}"/> for testing.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TId">The entity identifier type.</typeparam>
/// <remarks>
/// This is a test-only implementation that uses in-memory collections.
/// AOT/trimming warnings are suppressed as this is not intended for production.
/// </remarks>
[SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access", Justification = "TestKit is not intended for AOT/trimmed production scenarios")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling", Justification = "TestKit is not intended for AOT/trimmed production scenarios")]
public class FakeRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    private readonly Dictionary<TId, TEntity> _entities = [];

    /// <summary>
    /// Gets all stored entities.
    /// </summary>
    public IReadOnlyCollection<TEntity> AllEntities => _entities.Values.ToList().AsReadOnly();

    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TEntity>>(_entities.Values.ToList().AsReadOnly());
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return Task.FromResult<IReadOnlyList<TEntity>>(query.ToList().AsReadOnly());
    }

    /// <inheritdoc />
    public Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        return Task.FromResult(query.FirstOrDefault());
    }

    /// <inheritdoc />
    public Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        return Task.FromResult(query.Count());
    }

    /// <inheritdoc />
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_entities.Count);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        return Task.FromResult(query.Any());
    }

    /// <inheritdoc />
    public Task<PagedResult<TEntity>> GetPagedAsync(
        ISpecification<TEntity> specification,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _entities.Values.AsQueryable();

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        var totalCount = query.Count();

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        var items = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<TEntity>(items, pageNumber, pageSize, totalCount));
    }

    /// <inheritdoc />
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _entities[entity.Id] = entity;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            _entities[entity.Id] = entity;
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Update(TEntity entity)
    {
        _entities[entity.Id] = entity;
    }

    /// <inheritdoc />
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _entities[entity.Id] = entity;
        }
    }

    /// <inheritdoc />
    public void Remove(TEntity entity)
    {
        _entities.Remove(entity.Id);
    }

    /// <inheritdoc />
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _entities.Remove(entity.Id);
        }
    }

    /// <summary>
    /// Clears all stored entities.
    /// </summary>
    public void Clear()
    {
        _entities.Clear();
    }

    /// <summary>
    /// Seeds the repository with entities.
    /// </summary>
    public void Seed(params TEntity[] entities)
    {
        foreach (var entity in entities)
        {
            _entities[entity.Id] = entity;
        }
    }
}
