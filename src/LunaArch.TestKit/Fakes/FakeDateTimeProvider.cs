using LunaArch.Abstractions.Services;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="IDateTimeProvider"/> for testing.
/// </summary>
public sealed class FakeDateTimeProvider : IDateTimeProvider
{
    private DateTimeOffset _utcNow = DateTimeOffset.UtcNow;

    /// <inheritdoc />
    public DateTimeOffset UtcNow => _utcNow;

    /// <inheritdoc />
    public DateTimeOffset Now => _utcNow.ToLocalTime();

    /// <summary>
    /// Sets the current time.
    /// </summary>
    public void SetUtcNow(DateTimeOffset dateTime)
    {
        _utcNow = dateTime;
    }

    /// <summary>
    /// Advances the current time by the specified duration.
    /// </summary>
    public void Advance(TimeSpan duration)
    {
        _utcNow = _utcNow.Add(duration);
    }

    /// <summary>
    /// Resets to the actual current time.
    /// </summary>
    public void Reset()
    {
        _utcNow = DateTimeOffset.UtcNow;
    }
}
