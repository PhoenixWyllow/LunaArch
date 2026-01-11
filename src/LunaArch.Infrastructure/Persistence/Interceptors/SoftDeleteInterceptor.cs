using LunaArch.Abstractions.Primitives;
using LunaArch.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LunaArch.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that converts hard deletes to soft deletes for ISoftDeletable entities.
/// </summary>
public sealed class SoftDeleteInterceptor(
    IDateTimeProvider dateTimeProvider,
    ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ConvertToSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ConvertToSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ConvertToSoftDelete(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var entries = context.ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            // Convert hard delete to soft delete
            entry.State = EntityState.Modified;
            entry.Entity.SoftDelete(dateTimeProvider.UtcNow, currentUserService.UserId);
        }
    }
}
