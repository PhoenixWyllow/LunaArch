using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Results;
using LunaArch.TestKit.Fakes;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Vogen;
using Xunit;

namespace LunaArch.Application.Tests.Commands;

[ValueObject<Guid>]
public readonly partial struct OrderId;

[ValueObject<decimal>]
public readonly partial struct OrderAmount
{
    private static Validation Validate(decimal value)
    {
        if (value <= 0)
        {
            return Validation.Invalid("Order amount must be positive");
        }

        return Validation.Ok;
    }
}

public sealed record CreateOrderCommand(OrderAmount Amount) : ICommand<Result<OrderId>>;

public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Result<OrderId>>
{
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<OrderId>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating order with amount {Amount}", command.Amount.Value);

        var orderId = OrderId.From(Guid.NewGuid());
        return Task.FromResult(Result.Success(orderId));
    }
}

public class CommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccessResult()
    {
        var logger = Substitute.For<ILogger<CreateOrderCommandHandler>>();
        var handler = new CreateOrderCommandHandler(logger);

        var command = new CreateOrderCommand(OrderAmount.From(100m));

        var result = await handler.HandleAsync(command);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task HandleAsync_WithFakeDispatcher_CanDispatchCommand()
    {
        var fakeDispatcher = new FakeDispatcher();
        
        var orderId = OrderId.From(Guid.NewGuid());
        var expectedResult = Result.Success(orderId);
        fakeDispatcher.NextResult = expectedResult;

        var command = new CreateOrderCommand(OrderAmount.From(100m));
        var result = await fakeDispatcher.SendAsync<CreateOrderCommand, Result<OrderId>>(command);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(orderId);
        fakeDispatcher.GetDispatched<CreateOrderCommand>().ShouldContain(command);
    }
}
