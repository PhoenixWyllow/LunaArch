using LunaArch.Abstractions.Services;

namespace LunaArch.Infrastructure.Services;

/// <summary>
/// Default implementation of IDateTimeProvider using the system clock.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public DateTimeOffset Now => DateTimeOffset.Now;
}
