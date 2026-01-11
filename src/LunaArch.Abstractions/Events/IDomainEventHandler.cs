namespace LunaArch.Abstractions.Events;

/// <summary>
/// Interface for handling domain events.
/// </summary>
/// <typeparam name="TEvent">The type of domain event to handle.</typeparam>
// Justification: This follows the established Handler pattern (IRequestHandler, INotificationHandler, etc.)
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public interface IDomainEventHandler<in TEvent>
#pragma warning restore CA1711
    where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
