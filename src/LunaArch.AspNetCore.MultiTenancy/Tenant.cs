using LunaArch.AspNetCore.MultiTenancy.Abstractions;

namespace LunaArch.AspNetCore.MultiTenancy;

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
