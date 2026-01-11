using System.Collections.Frozen;
using LunaArch.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LunaArch.Infrastructure.Messaging;

/// <summary>
/// Delegate for dispatching a domain event to its handlers.
/// </summary>
/// <param name="domainEvent">The domain event to dispatch.</param>
/// <param name="serviceProvider">The service provider for resolving handlers.</param>
/// <param name="logger">The logger for error logging.</param>
/// <param name="cancellationToken">A cancellation token.</param>
/// <returns>A task representing the asynchronous operation.</returns>
// Justification: This IS a delegate type - the suffix accurately describes what it is.
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public delegate Task EventDispatchHandler(
    IDomainEvent domainEvent,
    IServiceProvider serviceProvider,
    ILogger logger,
    CancellationToken cancellationToken);
#pragma warning restore CA1711

/// <summary>
/// AOT-compatible registry for domain event dispatch delegates.
/// Register event types at startup to enable type-safe, reflection-free dispatch at runtime.
/// </summary>
/// <example>
/// <code>
/// // In DI registration:
/// services.AddLunaArch(builder =>
/// {
///     builder.AddDomainEvent&lt;OrderCreatedEvent&gt;();
///     builder.AddDomainEvent&lt;OrderCompletedEvent&gt;();
/// });
/// </code>
/// </example>
public sealed class DomainEventRegistry
{
    private readonly Dictionary<Type, EventDispatchHandler> _dispatchers = [];
    private FrozenDictionary<Type, EventDispatchHandler>? _frozenDispatchers;

    /// <summary>
    /// Registers a domain event type for AOT-compatible dispatch.
    /// Call this for each domain event type during startup.
    /// </summary>
    /// <typeparam name="TEvent">The domain event type to register.</typeparam>
    /// <returns>The registry for fluent chaining.</returns>
    public DomainEventRegistry Register<TEvent>() where TEvent : IDomainEvent
    {
        if (_frozenDispatchers is not null)
        {
            throw new InvalidOperationException(
                "Cannot register events after the registry has been frozen. " +
                "Ensure all events are registered during startup.");
        }

        _dispatchers[typeof(TEvent)] = CreateDispatcher<TEvent>();
        return this;
    }

    /// <summary>
    /// Freezes the registry for optimal runtime performance.
    /// Called automatically after DI configuration is complete.
    /// </summary>
    internal void Freeze()
    {
        _frozenDispatchers = _dispatchers.ToFrozenDictionary();
    }

    /// <summary>
    /// Gets the dispatch delegate for an event type.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="dispatcher">The dispatch delegate if found.</param>
    /// <returns>True if a dispatcher was found; otherwise false.</returns>
    public bool TryGetDispatcher(Type eventType, out EventDispatchHandler? dispatcher)
    {
        var dispatchers = _frozenDispatchers ?? _dispatchers.ToFrozenDictionary();
        return dispatchers.TryGetValue(eventType, out dispatcher);
    }

    /// <summary>
    /// Gets all registered event types.
    /// </summary>
    public IEnumerable<Type> RegisteredEventTypes =>
        _frozenDispatchers?.Keys ?? (IEnumerable<Type>)_dispatchers.Keys;

    private static EventDispatchHandler CreateDispatcher<TEvent>() where TEvent : IDomainEvent
    {
        return async (domainEvent, serviceProvider, logger, cancellationToken) =>
        {
            var typedEvent = (TEvent)domainEvent;
            var handlers = serviceProvider.GetServices<IDomainEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                try
                {
                    await handler.HandleAsync(typedEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Note: Using LoggerMessage would require a partial class which isn't possible in a delegate.
                    // This is acceptable since error logging is not in the hot path.
#pragma warning disable CA1848 // Use LoggerMessage delegates
                    logger.LogError(ex, "Error handling domain event {EventType} with handler {HandlerType}",
                        typeof(TEvent).Name, handler.GetType().Name);
#pragma warning restore CA1848
                    throw;
                }
            }
        };
    }
}
