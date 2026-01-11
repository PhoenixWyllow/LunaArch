using LunaArch.Infrastructure.MultiTenancy.Abstractions;

namespace LunaArch.Infrastructure.MultiTenancy;

/// <summary>
/// Provides access to the current tenant context.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    private static readonly AsyncLocal<ITenant?> _currentTenant = new();

    /// <inheritdoc />
    public ITenant? CurrentTenant => _currentTenant.Value;

    /// <inheritdoc />
    public string? TenantId => _currentTenant.Value?.Id;

    /// <summary>
    /// Sets the current tenant for the execution context.
    /// </summary>
    /// <param name="tenant">The tenant to set.</param>
    public static void SetCurrentTenant(ITenant? tenant)
    {
        _currentTenant.Value = tenant;
    }

    /// <summary>
    /// Clears the current tenant from the execution context.
    /// </summary>
    public static void ClearCurrentTenant()
    {
        _currentTenant.Value = null;
    }
}
