using LunaArch.Infrastructure.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;

namespace LunaArch.Infrastructure.MultiTenancy.Resolvers;

/// <summary>
/// Resolves tenant from HTTP header.
/// </summary>
public sealed class HeaderTenantResolver(IHttpContextAccessor httpContextAccessor) : ITenantResolver
{
    /// <summary>
    /// The default header name for tenant identification.
    /// </summary>
    public const string DefaultHeaderName = "X-Tenant-ID";

    /// <summary>
    /// Gets or sets the header name to look for.
    /// </summary>
    public string HeaderName { get; init; } = DefaultHeaderName;

    /// <inheritdoc />
    public Task<string?> ResolveTenantIdAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return Task.FromResult<string?>(null);
        }

        if (httpContext.Request.Headers.TryGetValue(HeaderName, out var tenantId)
            && !string.IsNullOrWhiteSpace(tenantId))
        {
            return Task.FromResult<string?>(tenantId.ToString());
        }

        return Task.FromResult<string?>(null);
    }
}
