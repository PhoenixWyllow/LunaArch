namespace LunaArch.Abstractions.Services;

/// <summary>
/// Interface for providing the current date and time.
/// Use this abstraction instead of DateTime.Now/UtcNow for testability.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTimeOffset Now { get; }
}
