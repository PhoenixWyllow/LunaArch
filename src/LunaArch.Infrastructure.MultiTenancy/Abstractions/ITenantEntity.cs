namespace LunaArch.Infrastructure.MultiTenancy.Abstractions;

/// <summary>
/// Interface for entities that belong to a specific tenant.
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    string TenantId { get; set; }
}
