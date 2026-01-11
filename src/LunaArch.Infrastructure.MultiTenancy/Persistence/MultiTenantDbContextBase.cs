using System.Diagnostics.CodeAnalysis;
using LunaArch.Infrastructure.MultiTenancy.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LunaArch.Infrastructure.MultiTenancy.Persistence;

/// <summary>
/// Base DbContext that supports multi-tenancy with automatic tenant filtering.
/// </summary>
/// <remarks>
/// EF Core is not fully AOT/trimming compatible. See https://aka.ms/efcore-docs-trimming
/// </remarks>
[SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access", Justification = "EF Core is not fully AOT compatible")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling", Justification = "EF Core is not fully AOT compatible")]
public abstract class MultiTenantDbContextBase : DbContext
{
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiTenantDbContextBase"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    /// <param name="tenantContext">The tenant context.</param>
    protected MultiTenantDbContextBase(
        DbContextOptions options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Gets the current tenant ID.
    /// </summary>
    protected string? CurrentTenantId => _tenantContext.TenantId;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter to all tenant entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .AddQueryFilter<ITenantEntity>(e => e.TenantId == CurrentTenantId);
            }
        }
    }
}

/// <summary>
/// Extension methods for ModelBuilder.
/// </summary>
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a query filter to an entity.
    /// </summary>
    public static void AddQueryFilter<T>(
        this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder entityTypeBuilder,
        System.Linq.Expressions.Expression<Func<T, bool>> filter) where T : class
    {
        // This is a simplified implementation
        // In practice, you'd need to compose filters if multiple are needed
        entityTypeBuilder.HasQueryFilter(filter);
    }
}
