namespace LunaArch.Abstractions.Services;

/// <summary>
/// Interface for accessing information about the current user.
/// Implement this to provide user context from your authentication system.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the unique identifier of the current user, or null if not authenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
