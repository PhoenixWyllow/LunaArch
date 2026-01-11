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

# API integration (choose one)
dotnet add package LunaArch.AspNetCore.MinimalApi   # For Minimal APIs
dotnet add package LunaArch.AspNetCore.FastEndpoints # For FastEndpoints

# Optional add-ons
dotnet add package LunaArch.AspNetCore.MultiTenancy
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
builder.Services.AddLunaArch<MyAppDbContext>(
    dbOptions =>
    {
        dbOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    },
    archConfig =>
    {
        // Register command handlers
        archConfig.AddCommandHandler<CreateOrderCommand, OrderId, CreateOrderCommandHandler>();
        
        // Register query handlers
        archConfig.AddQueryHandler<GetOrderQuery, OrderDto, GetOrderQueryHandler>();
        
        // Register domain events (required for non-generic dispatch)
        archConfig.AddDomainEvent<OrderCreatedEvent>();
        archConfig.AddDomainEvent<OrderCompletedEvent>();
        
        // Register domain event handlers
        archConfig.AddDomainEventHandler<OrderCreatedEvent, SendOrderConfirmationHandler>();
        
        // Register pipeline behaviors
        archConfig.AddBehavior<ValidationBehavior>();
    });

// Add ASP.NET Core integration
builder.Services.AddLunaArchAspNetCore();

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

### 5. Expose via API

LunaArch provides two API integration options. Choose one:

#### Option A: Minimal APIs

```csharp
using LunaArch.AspNetCore.MinimalApi;

// OrderEndpoints.cs
public class OrderEndpoints : IEndpointGroup
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");
        
        group.MapPost("/", async (CreateOrderCommand command, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var result = await dispatcher.SendAsync<CreateOrderCommand, Guid>(command, ct);
            return result.ToCreatedResponse($"/api/orders/{result.Value}");
        });

        group.MapGet("/{id:guid}", async (Guid id, IDispatcher dispatcher, CancellationToken ct) =>
        {
            var order = await dispatcher.QueryAsync<GetOrderQuery, OrderDto?>(new(id), ct);
            return order.ToResponse();
        });
    }
}

// Program.cs - add to service configuration
builder.Services.AddEndpointGroups<OrderEndpoints>();

// Program.cs - add after app.UseLunaArch()
app.MapEndpointGroups();
```

#### Option B: FastEndpoints

```csharp
using LunaArch.AspNetCore.FastEndpoints;

// CreateOrderEndpoint.cs
public class CreateOrderEndpoint : CommandEndpoint<CreateOrderCommand, ApiResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateOrderCommand req, CancellationToken ct)
    {
        var orderId = await SendCommandAsync<CreateOrderCommand, Guid>(req, ct);
        await SendCreatedAtAsync<GetOrderEndpoint>(new { id = orderId }, ApiResponse<Guid>.Ok(orderId), cancellation: ct);
    }
}

// GetOrderEndpoint.cs  
public class GetOrderEndpoint : QueryEndpoint<GetOrderRequest, ApiResponse<OrderDto>>
{
    public override void Configure()
    {
        Get("/api/orders/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        var order = await SendQueryAsync<GetOrderQuery, OrderDto?>(new(req.Id), ct);
        await (order is not null ? SendOkAsync(ApiResponse<OrderDto>.Ok(order), ct) : SendNotFoundAsync(ct));
    }
}

// Program.cs - add to service configuration
builder.Services.AddLunaArchFastEndpoints();

// Program.cs - add after app.UseLunaArch()
app.UseFastEndpoints();
```

## Next Steps

- [Architecture Overview](architecture/overview.md) - Understand the architecture
- [Domain Primitives](architecture/domain-primitives.md) - Learn about entities and value objects
- [CQRS and Messaging](architecture/cqrs.md) - Deep dive into commands and queries
- [Testing](architecture/testing.md) - Write tests using the TestKit
