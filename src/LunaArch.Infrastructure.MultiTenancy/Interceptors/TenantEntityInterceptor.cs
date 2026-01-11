using LunaArch.Infrastructure.MultiTenancy.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LunaArch.Infrastructure.MultiTenancy.Interceptors;

/// <summary>
/// Interceptor that automatically sets the tenant ID on new entities.
/// </summary>
public sealed class TenantEntityInterceptor(ITenantContext tenantContext)
    : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetTenantId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetTenantId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTenantId(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var tenantId = tenantContext.TenantId;

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return;
        }

        var entries = context.ChangeTracker
            .Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            entry.Entity.TenantId = tenantId;
        }
    }
}
