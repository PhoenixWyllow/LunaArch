namespace LunaArch.Abstractions.Events;

/// <summary>
/// Marker interface for domain events.
/// Domain events capture something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when this event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }
}
