# Layer Responsibilities

Each layer in LunaArch has specific responsibilities and rules about what it can and cannot do.

## Layer Diagram

```mermaid
graph TB
    subgraph Presentation["🌐 Presentation Layer"]
        direction TB
        P1[Controllers]
        P2[Middleware]
        P3[API Models/DTOs]
    end

    subgraph Application["⚙️ Application Layer"]
        direction TB
        A1[Commands & Queries]
        A2[Handlers]
        A3[DTOs/ViewModels]
        A4[Behaviors]
    end

    subgraph Domain["💎 Domain Layer"]
        direction TB
        D1[Aggregates/Entities]
        D2[Value Objects]
        D3[Domain Events]
        D4[Specifications]
        D5[Business Rules]
    end

    subgraph Infrastructure["🔧 Infrastructure Layer"]
        direction TB
        I1[Repositories]
        I2[DbContext]
        I3[External Services]
        I4[Dispatcher]
    end

    Presentation --> Application
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain

    style Presentation fill:#e3f2fd
    style Application fill:#fff8e1
    style Domain fill:#fce4ec
    style Infrastructure fill:#e8f5e9
```

## Abstractions Layer

**Purpose**: Define contracts and base types without any implementation details.

**Contains**:
- `Entity<TId>` - Base class for entities with identity
- `AggregateRoot<TId>` - Base class for aggregate roots
- `ValueObject` - Base class for value objects
- `IDomainEvent` - Interface for domain events
- `ICommand<T>`, `IQuery<T>` - CQRS interfaces
- `IRepository<T, TId>` - Repository contract
- `IUnitOfWork` - Unit of work contract

**Rules**:
- ✅ No external dependencies (pure .NET)
- ✅ Only interfaces and abstract base classes
- ❌ No implementations
- ❌ No business logic

```csharp
// Example: Pure abstraction
public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected init; } = default!;
    
    // No implementation of persistence, validation, etc.
}
```

## Domain Layer

**Purpose**: Encapsulate all business logic and domain rules.

**Contains**:
- Domain entities and aggregates
- Value objects
- Domain events
- Specifications
- Business rules
- Guard clauses

**Rules**:
- ✅ Contains all business logic
- ✅ Rich domain models (behavior + data)
- ✅ Domain events for state changes
- ❌ No infrastructure concerns (DB, HTTP, etc.)
- ❌ No dependency on Application layer
- ❌ Entities should never be exposed directly to outside world

```csharp
// Example: Rich domain model
public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderLine> _lines = [];
    
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; }

    public void AddLine(Product product, int quantity)
    {
        // Business rule: Cannot modify completed orders
        Guard.Against.InvalidOperation(
            Status == OrderStatus.Completed,
            "Cannot modify completed order");
        
        var line = new OrderLine(product, quantity);
        _lines.Add(line);
        
        RecalculateTotal();
        RaiseDomainEvent(new OrderLineAddedEvent(Id, line.Id));
    }

    public void Complete()
    {
        // Business rule validation
        this.CheckRule(new OrderMustHaveLinesRule(_lines));
        
        Status = OrderStatus.Completed;
        RaiseDomainEvent(new OrderCompletedEvent(Id));
    }
}
```

## Application Layer

**Purpose**: Orchestrate application use cases without business logic.

**Contains**:
- Commands and command handlers
- Queries and query handlers
- Application services
- DTOs for data transfer
- Pipeline behaviors
- Application exceptions

**Rules**:
- ✅ Coordinates domain operations
- ✅ Handles transactions
- ✅ Maps between DTOs and domain
- ❌ No business logic (delegate to domain)
- ❌ No infrastructure details
- ❌ Never expose domain entities directly

```csharp
// Example: Application handler
public sealed class CreateOrderCommandHandler(
    IRepository<Order, Guid> orderRepository,
    IRepository<Customer, Guid> customerRepository)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Load required entities
        var customer = await customerRepository.GetByIdAsync(
            command.CustomerId, 
            cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), command.CustomerId);

        // 2. Delegate business logic to domain
        var order = Order.Create(customer);

        foreach (var item in command.Items)
        {
            order.AddLine(item.ProductId, item.Quantity);
        }

        // 3. Persist changes
        await orderRepository.AddAsync(order, cancellationToken);

        // 4. Return result (ID, not entity)
        return order.Id;
    }
}
```

## Infrastructure Layer

**Purpose**: Implement technical concerns and external integrations.

**Contains**:
- Repository implementations
- DbContext configurations
- External service clients
- Dispatcher implementation
- Interceptors (auditing, soft delete)
- Message queue adapters

**Rules**:
- ✅ Implements abstractions from inner layers
- ✅ Contains all persistence logic
- ✅ Handles external communications
- ❌ No business logic
- ❌ Should be replaceable without affecting domain

```csharp
// Example: Repository implementation
public class RepositoryBase<TEntity, TId>(DbContext context)
    : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    protected DbSet<TEntity> DbSet => context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(
        TId id, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }
}
```

## Presentation Layer

**Purpose**: Handle HTTP requests and present data to clients.

**Contains**:
- API Controllers
- Middleware
- Request/Response DTOs
- API configuration
- Authentication/Authorization setup

**Rules**:
- ✅ Transform HTTP requests to commands/queries
- ✅ Handle HTTP-specific concerns
- ✅ API versioning and documentation
- ❌ No business logic
- ❌ No direct database access
- ❌ Never return domain entities

```csharp
// Example: Controller
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApiResponse<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Map request to command
        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)));

        // 2. Dispatch command
        var orderId = await dispatcher.SendAsync(command, cancellationToken);

        // 3. Return standardized response
        return Ok(ApiResponse<Guid>.Success(orderId));
    }
}
```

## Data Flow Diagram

```mermaid
flowchart LR
    subgraph Client
        REQ[HTTP Request]
    end

    subgraph Presentation
        CTRL[Controller]
        DTO1[Request DTO]
    end

    subgraph Application
        CMD[Command]
        HDL[Handler]
        DTO2[Response DTO]
    end

    subgraph Domain
        AGG[Aggregate]
        EVT[Domain Event]
    end

    subgraph Infrastructure
        REPO[Repository]
        DB[(Database)]
    end

    REQ --> CTRL
    CTRL --> DTO1
    DTO1 --> CMD
    CMD --> HDL
    HDL --> REPO
    REPO --> AGG
    AGG --> EVT
    REPO --> DB
    HDL --> DTO2
    DTO2 --> CTRL
```

## Cross-Cutting Concerns

Cross-cutting concerns are handled through pipeline behaviors:

```mermaid
graph LR
    REQ[Request] --> LOG[Logging Behavior]
    LOG --> VAL[Validation Behavior]
    VAL --> UOW[UnitOfWork Behavior]
    UOW --> HDL[Handler]
    HDL --> UOW
    UOW --> VAL
    VAL --> LOG
    LOG --> RES[Response]

    style LOG fill:#ffeb3b
    style VAL fill:#4caf50
    style UOW fill:#2196f3
```

## Next Steps

- [Domain Primitives](domain-primitives.md) - Deep dive into domain building blocks
- [CQRS and Messaging](cqrs.md) - Command/Query pattern details
- [Persistence](persistence.md) - Repository and Unit of Work patterns
