using LunaArch.Abstractions.Persistence;
using LunaArch.Abstractions.Primitives;
using LunaArch.Abstractions.Services;
using LunaArch.Infrastructure.Extensions;
using LunaArch.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Vogen;
using Xunit;

namespace LunaArch.Infrastructure.Tests.Extensions;

[ValueObject<Guid>]
public readonly partial struct TestEntityId;

public sealed class TestEntity : AggregateRoot<TestEntityId>
{
    public string Name { get; init; } = string.Empty;
}

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContextBase(options)
{
    public DbSet<TestEntity> TestEntities => Set<TestEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(builder =>
        {
            builder.ToTable("TestEntities");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired();
        });
    }
}

public class ServiceCollectionExtensionsTests
{
    private static ServiceCollection CreateServicesWithMocks()
    {
        var services = new ServiceCollection();

        // Mock required dependencies
        services.AddSingleton(Substitute.For<ICurrentUserService>());
        services.AddSingleton(Substitute.For<IDateTimeProvider>());

        return services;
    }

    [Fact]
    public void AddLunaArch_RegistersDbContextBaseType()
    {
        var services = CreateServicesWithMocks();

        services.AddLunaArch<TestDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        var serviceProvider = services.BuildServiceProvider();

        var dbContext = serviceProvider.GetService<DbContext>();

        dbContext.ShouldNotBeNull();
        dbContext.ShouldBeOfType<TestDbContext>();
    }

    [Fact]
    public void AddLunaArch_AllowsRepositoryBaseResolution()
    {
        var services = CreateServicesWithMocks();

        services.AddLunaArch<TestDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        // Register generic repository (as documented in usage)
        services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
        services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepositoryBase<,>));

        var serviceProvider = services.BuildServiceProvider();

        // This should not throw InvalidOperationException
        var repository = serviceProvider.GetService<IRepository<TestEntity, TestEntityId>>();

        repository.ShouldNotBeNull();
        repository.ShouldBeOfType<RepositoryBase<TestEntity, TestEntityId>>();
    }

    [Fact]
    public void AddLunaArch_AllowsReadRepositoryBaseResolution()
    {
        var services = CreateServicesWithMocks();

        services.AddLunaArch<TestDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb");
        });

        // Register generic repository (as documented in usage)
        services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepositoryBase<,>));

        var serviceProvider = services.BuildServiceProvider();

        // This should not throw InvalidOperationException
        var repository = serviceProvider.GetService<IReadRepository<TestEntity, TestEntityId>>();

        repository.ShouldNotBeNull();
        repository.ShouldBeOfType<ReadRepositoryBase<TestEntity, TestEntityId>>();
    }
}
