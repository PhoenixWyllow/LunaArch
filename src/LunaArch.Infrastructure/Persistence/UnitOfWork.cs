using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LunaArch.Infrastructure.Persistence;

/// <summary>
/// Implementation of the Unit of Work pattern.
/// Coordinates saving changes and managing transactions.
/// </summary>
/// <typeparam name="TContext">The DbContext type.</typeparam>
public class UnitOfWork<TContext>(
    TContext context,
    IDomainEventDispatcher? domainEventDispatcher = null) : IUnitOfWork
    where TContext : DbContextBase
{
    private IDbContextTransaction? _currentTransaction;

    /// <summary>
    /// Gets the DbContext.
    /// </summary>
    protected TContext Context { get; } = context;

    /// <inheritdoc />
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        if (domainEventDispatcher is not null)
        {
            var domainEvents = Context.GetDomainEvents();
            Context.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
            }
        }

        return await Context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await Context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await Context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <inheritdoc />
    public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}
