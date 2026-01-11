using LunaArch.Abstractions.Events;
using LunaArch.TestKit.Fakes;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Vogen;
using Xunit;

namespace LunaArch.Application.Tests.Events;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public sealed record OrderCompletedEvent(OrderId OrderId, DateTimeOffset CompletedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}

public sealed class OrderCompletedEventHandler : IDomainEventHandler<OrderCompletedEvent>
{
    private readonly ILogger<OrderCompletedEventHandler> _logger;

    public OrderCompletedEventHandler(ILogger<OrderCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Order {OrderId} completed at {CompletedAt}",
            domainEvent.OrderId.Value,
            domainEvent.CompletedAt);

        return Task.CompletedTask;
    }
}

public class DomainEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidEvent_HandlesSuccessfully()
    {
        var logger = Substitute.For<ILogger<OrderCompletedEventHandler>>();
        var handler = new OrderCompletedEventHandler(logger);

        var orderId = OrderId.From(Guid.NewGuid());
        var @event = new OrderCompletedEvent(orderId, DateTimeOffset.UtcNow);

        await handler.HandleAsync(@event);

        // No exception means success
    }

    [Fact]
    public async Task HandleAsync_WithFakeDispatcher_CanDispatchEvent()
    {
        var fakeDispatcher = new FakeDomainEventDispatcher();

        var orderId = OrderId.From(Guid.NewGuid());
        var @event = new OrderCompletedEvent(orderId, DateTimeOffset.UtcNow);

        await fakeDispatcher.DispatchAsync(@event);

        fakeDispatcher.DispatchedEvents.ShouldContain(@event);
    }

    [Fact]
    public async Task HandleAsync_WithStronglyTypedDispatch_Works()
    {
        var fakeDispatcher = new FakeDomainEventDispatcher();

        var orderId = OrderId.From(Guid.NewGuid());
        var @event = new OrderCompletedEvent(orderId, DateTimeOffset.UtcNow);

        // Test strongly-typed generic overload
        await fakeDispatcher.DispatchAsync<OrderCompletedEvent>(@event);

        fakeDispatcher.DispatchedEvents.ShouldContain(@event);
    }
}
