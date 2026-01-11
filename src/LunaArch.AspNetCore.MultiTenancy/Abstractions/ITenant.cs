namespace LunaArch.AspNetCore.MultiTenancy.Abstractions;

/// <summary>
/// Represents a tenant in a multi-tenant application.
/// </summary>
public interface ITenant
{
    /// <summary>
    /// Gets the unique identifier for the tenant.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the tenant.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the connection string for the tenant's database (if using database-per-tenant strategy).
    /// </summary>
    string? ConnectionString { get; }

    /// <summary>
    /// Gets a value indicating whether the tenant is active.
    /// </summary>
    bool IsActive { get; }
}
