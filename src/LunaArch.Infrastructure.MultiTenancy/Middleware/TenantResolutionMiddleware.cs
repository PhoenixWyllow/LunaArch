using LunaArch.Infrastructure.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LunaArch.Infrastructure.MultiTenancy.Middleware;

/// <summary>
/// Middleware that resolves the current tenant for each request.
/// </summary>
public sealed partial class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="tenantResolver">The tenant resolver.</param>
    /// <param name="tenantStore">The tenant store.</param>
    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ITenantStore tenantStore)
    {
        try
        {
            var tenantId = await tenantResolver.ResolveTenantIdAsync();

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var tenant = await tenantStore.GetTenantAsync(tenantId);

                if (tenant is not null && tenant.IsActive)
                {
                    TenantContext.SetCurrentTenant(tenant);
                    LogTenantResolved(logger, tenant.Id, tenant.Name);
                }
                else
                {
                    LogTenantNotFound(logger, tenantId);
                }
            }

            await next(context);
        }
        finally
        {
            TenantContext.ClearCurrentTenant();
        }
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Tenant resolved: {TenantId} ({TenantName})")]
    private static partial void LogTenantResolved(ILogger logger, string tenantId, string tenantName);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Tenant not found or inactive: {TenantId}")]
    private static partial void LogTenantNotFound(ILogger logger, string tenantId);
}
