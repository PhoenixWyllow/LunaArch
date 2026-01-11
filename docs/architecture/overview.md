# Architecture Overview

LunaArch implements Clean Architecture with Domain-Driven Design patterns, providing a structured approach to building maintainable enterprise applications.

## Architecture Diagram

```mermaid
graph TB
    subgraph Presentation["Presentation Layer"]
        API[ASP.NET Core API]
        MW[Middleware]
    end

    subgraph Application["Application Layer"]
        CMD[Commands]
        QRY[Queries]
        HDL[Handlers]
        BHV[Behaviors]
    end

    subgraph Domain["Domain Layer"]
        AGG[Aggregates]
        ENT[Entities]
        VO[Value Objects]
        EVT[Domain Events]
        SPEC[Specifications]
        RULES[Business Rules]
    end

    subgraph Infrastructure["Infrastructure Layer"]
        DB[(Database)]
        REPO[Repositories]
        UOW[Unit of Work]
        DISP[Dispatcher]
        EFC[EF Core]
    end

    API --> MW
    MW --> DISP
    DISP --> HDL
    HDL --> CMD
    HDL --> QRY
    HDL --> BHV

    HDL --> REPO
    REPO --> AGG
    REPO --> SPEC
    
    AGG --> ENT
    AGG --> VO
    AGG --> EVT
    AGG --> RULES

    REPO --> EFC
    EFC --> DB
    UOW --> EFC

    style Presentation fill:#e1f5fe
    style Application fill:#fff3e0
    style Domain fill:#f3e5f5
    style Infrastructure fill:#e8f5e9
```

## Dependency Flow

```mermaid
graph LR
    subgraph External["External"]
        ASPNET[ASP.NET Core]
        EFC[EF Core]
    end

    subgraph LunaArch["LunaArch Framework"]
        ABS[Abstractions]
        DOM[Domain]
        APP[Application]
        INF[Infrastructure]
        ASP[AspNetCore]
        MT[MultiTenancy]
        TK[TestKit]
    end

    DOM --> ABS
    APP --> ABS
    INF --> DOM
    INF --> APP
    INF --> EFC
    ASP --> INF
    ASP --> ASPNET
    MT --> INF
    MT --> ASPNET
    TK --> ABS

    style ABS fill:#4caf50,color:#fff
    style DOM fill:#2196f3,color:#fff
    style APP fill:#ff9800,color:#fff
    style INF fill:#9c27b0,color:#fff
    style ASP fill:#f44336,color:#fff
    style MT fill:#f44336,color:#fff
```

## Layer Responsibilities

### Abstractions Layer
The foundation layer containing interfaces and base types with **zero external dependencies**:
- Domain primitives (`Entity`, `AggregateRoot`, `ValueObject`)
- Event interfaces (`IDomainEvent`, `IDomainEventHandler`)
- Messaging contracts (`ICommand`, `IQuery`, `IDispatcher`)
- Persistence abstractions (`IRepository`, `IUnitOfWork`)

### Domain Layer
Contains domain-specific building blocks:
- `Specification<T>` for query encapsulation
- Business rule validation
- Guard clauses for input validation

### Application Layer
Orchestrates application workflows:
- Application-specific exceptions
- Pipeline behaviors for cross-cutting concerns
- Command/Query validation

### Infrastructure Layer
Implements technical concerns:
- `Dispatcher` for command/query routing
- `RepositoryBase` with EF Core
- `DbContextBase` with domain event dispatching
- Interceptors for auditing and soft delete

### AspNetCore Layer
Web API integration:
- Exception handling middleware
- Correlation ID middleware
- Standard API response format
- Service registration extensions

## Design Principles

### 1. Dependency Inversion
All dependencies point inward toward the domain:
```
Presentation → Application → Domain ← Infrastructure
```

### 2. Explicit Dependencies
No hidden dependencies or service location. All dependencies are explicitly injected:
```csharp
public class OrderHandler(
    IRepository<Order, Guid> repository,  // Explicit
    IDateTimeProvider dateTime)           // Explicit
{
}
```

### 3. AOT Compatibility
Designed for Ahead-of-Time compilation:
- Explicit handler registration (no assembly scanning)
- Minimal reflection usage
- Source generators friendly

### 4. Testability
Every component is designed for testing:
- Interface-based abstractions
- TestKit with fake implementations
- No static dependencies

## Request Flow

```mermaid
sequenceDiagram
    participant C as Controller
    participant D as Dispatcher
    participant B as Pipeline Behaviors
    participant H as Handler
    participant R as Repository
    participant DB as Database

    C->>D: SendAsync(command)
    D->>B: Execute behaviors
    B->>H: HandleAsync(command)
    H->>R: AddAsync(entity)
    R->>DB: SaveChanges
    DB-->>R: Success
    R-->>H: Return
    H-->>B: Result
    B-->>D: Result
    D-->>C: Result
```

## Next Steps

- [Layer Responsibilities](layers.md) - Detailed layer documentation
- [Domain Primitives](domain-primitives.md) - Entities, Value Objects, Aggregates
- [CQRS and Messaging](cqrs.md) - Commands, Queries, Dispatcher
