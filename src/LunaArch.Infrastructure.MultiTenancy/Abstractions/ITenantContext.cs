namespace LunaArch.Infrastructure.MultiTenancy.Abstractions;

/// <summary>
/// Provides access to the current tenant.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant, or null if no tenant is resolved.
    /// </summary>
    ITenant? CurrentTenant { get; }

    /// <summary>
    /// Gets the current tenant ID, or null if no tenant is resolved.
    /// </summary>
    string? TenantId { get; }
}
