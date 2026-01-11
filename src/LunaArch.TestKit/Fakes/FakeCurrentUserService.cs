using LunaArch.Abstractions.Services;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="ICurrentUserService"/> for testing.
/// </summary>
public sealed class FakeCurrentUserService : ICurrentUserService
{
    /// <inheritdoc />
    public string? UserId { get; set; }

    /// <inheritdoc />
    public string? UserName { get; set; }

    /// <inheritdoc />
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Configures the service as an authenticated user.
    /// </summary>
    public FakeCurrentUserService AsUser(string userId, string? userName = null)
    {
        UserId = userId;
        UserName = userName ?? $"User-{userId}";
        IsAuthenticated = true;
        return this;
    }

    /// <summary>
    /// Configures the service as an anonymous user.
    /// </summary>
    public FakeCurrentUserService AsAnonymous()
    {
        UserId = null;
        UserName = null;
        IsAuthenticated = false;
        return this;
    }

    /// <summary>
    /// Resets to default anonymous state.
    /// </summary>
    public void Reset()
    {
        AsAnonymous();
    }
}
