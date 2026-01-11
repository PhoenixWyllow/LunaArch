namespace LunaArch.Infrastructure.MultiTenancy.Abstractions;

/// <summary>
/// Resolves the current tenant from the request context.
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Resolves the tenant identifier from the current context.
    /// </summary>
    /// <returns>The tenant identifier, or null if not resolved.</returns>
    Task<string?> ResolveTenantIdAsync();
}
