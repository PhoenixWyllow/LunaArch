using LunaArch.Infrastructure.MultiTenancy.Abstractions;

namespace LunaArch.Infrastructure.MultiTenancy;

/// <summary>
/// Default implementation of a tenant.
/// </summary>
public sealed class Tenant : ITenant
{
    /// <inheritdoc />
    public required string Id { get; init; }

    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public string? ConnectionString { get; init; }

    /// <inheritdoc />
    public bool IsActive { get; init; } = true;
}
