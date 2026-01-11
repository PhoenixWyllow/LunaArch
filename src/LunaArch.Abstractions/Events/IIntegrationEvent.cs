namespace LunaArch.Abstractions.Events;

/// <summary>
/// Marker interface for integration events.
/// Integration events are used to communicate between bounded contexts or external systems.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when this event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets the type name of the event for serialization purposes.
    /// </summary>
    string EventType { get; }
}
