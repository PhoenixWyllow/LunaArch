# Getting Started with LunaArch

This guide walks you through setting up LunaArch in a new project.

## Installation

Add the NuGet packages to your project:

```bash
# Core packages (choose based on your layer)
dotnet add package LunaArch.Abstractions
dotnet add package LunaArch.Domain
dotnet add package LunaArch.Application
dotnet add package LunaArch.Infrastructure
dotnet add package LunaArch.AspNetCore

# Optional add-ons
dotnet add package LunaArch.Infrastructure.MultiTenancy
dotnet add package LunaArch.TestKit
```

## Project Structure

A typical project using LunaArch follows this structure:

```
MyApp/
├── src/
│   ├── MyApp.Domain/           # References: LunaArch.Domain
│   ├── MyApp.Application/      # References: LunaArch.Application, MyApp.Domain
│   ├── MyApp.Infrastructure/   # References: LunaArch.Infrastructure, MyApp.Application
│   └── MyApp.Api/              # References: LunaArch.AspNetCore, MyApp.Infrastructure
└── tests/
    ├── MyApp.Domain.Tests/     # References: LunaArch.TestKit
    └── MyApp.Application.Tests/
```

## Basic Setup

### 1. Configure Services

In your `Program.cs`:

```csharp
using LunaArch.Infrastructure.Extensions;
using LunaArch.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add LunaArch services with your DbContext
builder.Services.AddLunaArch<MyAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add ASP.NET Core integration
builder.Services.AddLunaArchAspNetCore();

// Register your handlers
builder.Services.AddDispatcher(dispatcher =>
{
    dispatcher.RegisterCommandHandler<CreateOrderCommand, Guid, CreateOrderCommandHandler>();
    dispatcher.RegisterQueryHandler<GetOrderQuery, OrderDto, GetOrderQueryHandler>();
});

var app = builder.Build();

// Use LunaArch middleware
app.UseLunaArch();

app.Run();
```

### 2. Define a Domain Entity

```csharp
using LunaArch.Abstractions.Primitives;
using LunaArch.Domain.Rules;

public sealed class Order : AggregateRoot<Guid>, IAuditableEntity
{
    public string CustomerName { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    private Order() { } // EF Core constructor

    public static Order Create(string customerName, decimal totalAmount)
    {
        Guard.Against.NullOrWhiteSpace(customerName);
        Guard.Against.NegativeOrZero(totalAmount);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = customerName,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending
        };

        order.RaiseDomainEvent(new OrderCreatedEvent(order.Id, customerName));

        return order;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Only pending orders can be completed.");
        }

        Status = OrderStatus.Completed;
        RaiseDomainEvent(new OrderCompletedEvent(Id));
    }
}
```

### 3. Create a Command and Handler

```csharp
using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Persistence;

public sealed record CreateOrderCommand(
    string CustomerName,
    decimal TotalAmount) : ICommand<Guid>;

public sealed class CreateOrderCommandHandler(
    IRepository<Order, Guid> repository)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = Order.Create(command.CustomerName, command.TotalAmount);

        await repository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

### 4. Create a Query and Handler

```csharp
using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Persistence;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderDto?>;

public sealed record OrderDto(Guid Id, string CustomerName, decimal TotalAmount, string Status);

public sealed class GetOrderQueryHandler(
    IReadRepository<Order, Guid> repository)
    : IQueryHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> HandleAsync(
        GetOrderQuery query,
        CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(query.OrderId, cancellationToken);

        return order is null
            ? null
            : new OrderDto(order.Id, order.CustomerName, order.TotalAmount, order.Status.ToString());
    }
}
```

### 5. Use in Controller

```csharp
using LunaArch.Abstractions.Messaging;
using LunaArch.AspNetCore.Results;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderId = await dispatcher.SendAsync(command, cancellationToken);
        return ApiResponse<Guid>.Success(orderId);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await dispatcher.SendAsync(new GetOrderQuery(id), cancellationToken);

        return order is null
            ? NotFound()
            : ApiResponse<OrderDto>.Success(order);
    }
}
```

## Next Steps

- [Architecture Overview](architecture/overview.md) - Understand the architecture
- [Domain Primitives](architecture/domain-primitives.md) - Learn about entities and value objects
- [CQRS and Messaging](architecture/cqrs.md) - Deep dive into commands and queries
- [Testing](architecture/testing.md) - Write tests using the TestKit
