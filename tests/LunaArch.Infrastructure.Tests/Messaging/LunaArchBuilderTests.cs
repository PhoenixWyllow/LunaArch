using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Messaging;
using LunaArch.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace LunaArch.Infrastructure.Tests.Messaging;

#region Test types

public sealed record TestCommand(string Value) : ICommand<string>;

public sealed class TestCommandHandler : ICommandHandler<TestCommand, string>
{
    public Task<string> HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
        => Task.FromResult(command.Value);
}

public sealed record TestQuery(int Id) : IQuery<string>;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, string>
{
    public Task<string> HandleAsync(TestQuery query, CancellationToken cancellationToken = default)
        => Task.FromResult($"Result-{query.Id}");
}

public sealed record TestDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

public sealed class TestDomainEventHandler : IDomainEventHandler<TestDomainEvent>
{
    public Task HandleAsync(TestDomainEvent domainEvent, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public sealed class TestBehavior : IPipelineBehavior<TestCommand, string>
{
    public Task<string> HandleAsync(
        TestCommand request,
        PipelineDelegate<string> continuation,
        CancellationToken cancellationToken = default)
        => continuation(cancellationToken);
}

#endregion

public class LunaArchBuilderTests
{
    private static (LunaArchBuilder builder, IServiceCollection services) CreateBuilder()
    {
        var services = new ServiceCollection();
        var registry = new DomainEventRegistry();
        var builder = new LunaArchBuilder(services, registry);

        return (builder, services);
    }

    [Fact]
    public void LunaArchBuilder_ImplementsILunaArchBuilder()
    {
        var (builder, _) = CreateBuilder();

        builder.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddCommandHandler_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddCommandHandler<TestCommand, string, TestCommandHandler>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddCommandHandler_RegistersHandler_InServiceCollection()
    {
        var (builder, services) = CreateBuilder();

        builder.AddCommandHandler<TestCommand, string, TestCommandHandler>();

        var provider = services.BuildServiceProvider();
        var handler = provider.GetService<ICommandHandler<TestCommand, string>>();
        handler.ShouldNotBeNull();
        handler.ShouldBeOfType<TestCommandHandler>();
    }

    [Fact]
    public void AddQueryHandler_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddQueryHandler<TestQuery, string, TestQueryHandler>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddQueryHandler_RegistersHandler_InServiceCollection()
    {
        var (builder, services) = CreateBuilder();

        builder.AddQueryHandler<TestQuery, string, TestQueryHandler>();

        var provider = services.BuildServiceProvider();
        var handler = provider.GetService<IQueryHandler<TestQuery, string>>();
        handler.ShouldNotBeNull();
        handler.ShouldBeOfType<TestQueryHandler>();
    }

    [Fact]
    public void AddDomainEvent_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddDomainEvent<TestDomainEvent>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddDomainEventHandler_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddDomainEventHandler<TestDomainEvent, TestDomainEventHandler>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddDomainEventHandler_RegistersHandler_InServiceCollection()
    {
        var (builder, services) = CreateBuilder();

        builder.AddDomainEventHandler<TestDomainEvent, TestDomainEventHandler>();

        var provider = services.BuildServiceProvider();
        var handler = provider.GetService<IDomainEventHandler<TestDomainEvent>>();
        handler.ShouldNotBeNull();
        handler.ShouldBeOfType<TestDomainEventHandler>();
    }

    [Fact]
    public void AddBehavior_Generic_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddBehavior<TestBehavior>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void AddBehavior_Generic_RegistersBehavior_InServiceCollection()
    {
        var (builder, services) = CreateBuilder();

        builder.AddBehavior<TestBehavior>();

        var provider = services.BuildServiceProvider();
        var behavior = provider.GetService<IPipelineBehavior<TestCommand, string>>();
        behavior.ShouldNotBeNull();
        behavior.ShouldBeOfType<TestBehavior>();
    }

    [Fact]
    public void AddBehavior_Typed_ReturnsILunaArchBuilder_ForFluentChaining()
    {
        var (builder, _) = CreateBuilder();

        ILunaArchBuilder result = builder.AddBehavior<TestCommand, string, TestBehavior>();

        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ILunaArchBuilder>();
    }

    [Fact]
    public void FluentChaining_ThroughInterface_RegistersAllHandlers()
    {
        var (builder, services) = CreateBuilder();

#pragma warning disable CA1859 // Intentionally using interface type to verify fluent chaining via ILunaArchBuilder
        ILunaArchBuilder interfaceBuilder = builder;
#pragma warning restore CA1859
        interfaceBuilder
            .AddCommandHandler<TestCommand, string, TestCommandHandler>()
            .AddQueryHandler<TestQuery, string, TestQueryHandler>()
            .AddDomainEventHandler<TestDomainEvent, TestDomainEventHandler>()
            .AddBehavior<TestBehavior>();

        var provider = services.BuildServiceProvider();
        provider.GetService<ICommandHandler<TestCommand, string>>().ShouldNotBeNull();
        provider.GetService<IQueryHandler<TestQuery, string>>().ShouldNotBeNull();
        provider.GetService<IDomainEventHandler<TestDomainEvent>>().ShouldNotBeNull();
        provider.GetService<IPipelineBehavior<TestCommand, string>>().ShouldNotBeNull();
    }
}
