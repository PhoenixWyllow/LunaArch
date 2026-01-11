using LunaArch.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LunaArch.Infrastructure.Messaging;

/// <summary>
/// AOT-compatible domain event dispatcher using a pre-registered delegate registry.
/// </summary>
/// <remarks>
/// <para>
/// This dispatcher uses a registry pattern for AOT compatibility. Domain event types
/// must be registered during startup using <see cref="DomainEventRegistry"/>.
/// </para>
/// <para>
/// For unregistered events, use the strongly-typed <see cref="DispatchAsync{TEvent}"/> overload.
/// </para>
/// </remarks>
public sealed partial class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    DomainEventRegistry registry,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    /// <inheritdoc />
    public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var handlers = serviceProvider.GetServices<IDomainEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                LogHandlerError(logger, typeof(TEvent).Name, handler.GetType().Name, ex);
                throw;
            }
        }
    }

    /// <inheritdoc />
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var eventType = domainEvent.GetType();

        if (!registry.TryGetDispatcher(eventType, out var dispatcher) || dispatcher is null)
        {
            throw new InvalidOperationException(
                $"No dispatcher registered for domain event type '{eventType.Name}'. " +
                $"Register the event type during startup using builder.AddDomainEvent<{eventType.Name}>().");
        }

        await dispatcher(domainEvent, serviceProvider, logger, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await DispatchAsync(domainEvent, cancellationToken);
        }
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error handling domain event {EventType} with handler {HandlerType}")]
    private static partial void LogHandlerError(ILogger logger, string eventType, string handlerType, Exception ex);
}
