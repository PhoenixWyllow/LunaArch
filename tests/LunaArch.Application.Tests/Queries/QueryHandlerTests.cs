using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Results;
using LunaArch.TestKit.Fakes;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Vogen;
using Xunit;

namespace LunaArch.Application.Tests.Queries;

[ValueObject<Guid>]
public readonly partial struct OrderId;

public sealed record OrderDto(Guid Id, decimal Amount, string Status);

public sealed record GetOrderQuery(OrderId OrderId) : IQuery<Result<OrderDto>>;

public sealed class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, Result<OrderDto>>
{
    private readonly ILogger<GetOrderQueryHandler> _logger;

    public GetOrderQueryHandler(ILogger<GetOrderQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<OrderDto>> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting order {OrderId}", query.OrderId.Value);

        // Simulate finding an order
        var dto = new OrderDto(query.OrderId.Value, 100m, "Processing");
        return Task.FromResult(Result.Success(dto));
    }
}

public class QueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidQuery_ReturnsSuccessResult()
    {
        var logger = Substitute.For<ILogger<GetOrderQueryHandler>>();
        var handler = new GetOrderQueryHandler(logger);

        var orderId = OrderId.From(Guid.NewGuid());
        var query = new GetOrderQuery(orderId);

        var result = await handler.HandleAsync(query);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(orderId.Value);
        result.Value.Amount.ShouldBe(100m);
        result.Value.Status.ShouldBe("Processing");
    }

    [Fact]
    public async Task HandleAsync_WithFakeDispatcher_CanDispatchQuery()
    {
        var fakeDispatcher = new FakeDispatcher();
        
        var orderId = OrderId.From(Guid.NewGuid());
        var expectedDto = new OrderDto(orderId.Value, 100m, "Completed");
        var expectedResult = Result.Success(expectedDto);
        fakeDispatcher.NextResult = expectedResult;

        var query = new GetOrderQuery(orderId);
        var result = await fakeDispatcher.QueryAsync<GetOrderQuery, Result<OrderDto>>(query);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedDto);
        fakeDispatcher.GetDispatched<GetOrderQuery>().ShouldContain(query);
    }
}
