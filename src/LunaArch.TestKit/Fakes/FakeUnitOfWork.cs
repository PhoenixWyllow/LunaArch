using LunaArch.Abstractions.Persistence;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="IUnitOfWork"/> for testing.
/// </summary>
public sealed class FakeUnitOfWork : IUnitOfWork
{
    private int _saveChangesCount;
    private int _transactionCount;
    private bool _hasActiveTransaction;

    /// <summary>
    /// Gets the number of times SaveChangesAsync was called.
    /// </summary>
    public int SaveChangesCount => _saveChangesCount;

    /// <summary>
    /// Gets whether there is an active transaction.
    /// </summary>
    public bool HasActiveTransaction => _hasActiveTransaction;

    /// <summary>
    /// Gets or sets whether SaveChangesAsync should throw an exception.
    /// </summary>
    public Exception? ExceptionToThrow { get; set; }

    /// <summary>
    /// Gets or sets the result to return from SaveChangesAsync.
    /// </summary>
    public int ResultToReturn { get; set; } = 1;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        _saveChangesCount++;
        return Task.FromResult(ResultToReturn);
    }

    /// <inheritdoc />
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _hasActiveTransaction = true;
        _transactionCount++;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        _hasActiveTransaction = false;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets the fake unit of work state.
    /// </summary>
    public void Reset()
    {
        _saveChangesCount = 0;
        _transactionCount = 0;
        _hasActiveTransaction = false;
        ExceptionToThrow = null;
        ResultToReturn = 1;
    }

    /// <summary>
    /// Verifies that SaveChangesAsync was called a specific number of times.
    /// </summary>
    public bool WasSavedTimes(int times) => _saveChangesCount == times;

    /// <summary>
    /// Verifies that SaveChangesAsync was called at least once.
    /// </summary>
    public bool WasSaved() => _saveChangesCount > 0;
}
