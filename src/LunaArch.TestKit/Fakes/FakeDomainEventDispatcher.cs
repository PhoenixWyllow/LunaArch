using LunaArch.Abstractions.Events;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="IDomainEventDispatcher"/> for testing.
/// </summary>
public sealed class FakeDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly List<IDomainEvent> _dispatchedEvents = [];

    /// <summary>
    /// Gets all dispatched domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DispatchedEvents => _dispatchedEvents.AsReadOnly();

    /// <inheritdoc />
    public Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        _dispatchedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _dispatchedEvents.Add(domainEvent);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        _dispatchedEvents.AddRange(domainEvents);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all dispatched events.
    /// </summary>
    public void Clear()
    {
        _dispatchedEvents.Clear();
    }

    /// <summary>
    /// Gets dispatched events of a specific type.
    /// </summary>
    public IEnumerable<T> GetEvents<T>() where T : IDomainEvent => _dispatchedEvents.OfType<T>();

    /// <summary>
    /// Checks if an event of a specific type was dispatched.
    /// </summary>
    public bool WasDispatched<T>() where T : IDomainEvent => _dispatchedEvents.OfType<T>().Any();

    /// <summary>
    /// Checks if an event matching a predicate was dispatched.
    /// </summary>
    public bool WasDispatched<T>(Func<T, bool> predicate) where T : IDomainEvent =>
        _dispatchedEvents.OfType<T>().Any(predicate);
}
