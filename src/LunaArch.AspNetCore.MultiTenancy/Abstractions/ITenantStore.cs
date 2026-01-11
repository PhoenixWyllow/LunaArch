namespace LunaArch.AspNetCore.MultiTenancy.Abstractions;

/// <summary>
/// Store for retrieving tenant information.
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Gets a tenant by its identifier.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The tenant if found; otherwise, null.</returns>
    Task<ITenant?> GetTenantAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tenants.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of all tenants.</returns>
    Task<IReadOnlyList<ITenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
}
