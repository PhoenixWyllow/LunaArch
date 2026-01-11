using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace LunaArch.Infrastructure.Persistence;

/// <summary>
/// Base DbContext that provides common functionality for domain-driven design.
/// Inherit from this class in your application's DbContext.
/// </summary>
/// <example>
/// <code>
/// public sealed class ApplicationDbContext : DbContextBase
/// {
///     public DbSet&lt;Order&gt; Orders => Set&lt;Order&gt;();
///
///     public ApplicationDbContext(DbContextOptions&lt;ApplicationDbContext&gt; options)
///         : base(options) { }
/// }
/// </code>
/// </example>
/// <remarks>
/// EF Core is not fully AOT/trimming compatible. See https://aka.ms/efcore-docs-trimming
/// </remarks>
[SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access", Justification = "EF Core is not fully AOT compatible")]
[SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling", Justification = "EF Core is not fully AOT compatible")]
public abstract class DbContextBase : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextBase"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    protected DbContextBase(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Gets domain events from all tracked aggregate roots.
    /// </summary>
    /// <returns>A collection of domain events.</returns>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        return domainEvents;
    }

    /// <summary>
    /// Clears domain events from all tracked aggregate roots.
    /// </summary>
    public void ClearDomainEvents()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply soft delete filter to all ISoftDeletable entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression CreateSoftDeleteFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        var condition = System.Linq.Expressions.Expression.Equal(
            property,
            System.Linq.Expressions.Expression.Constant(false));

        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }
}
