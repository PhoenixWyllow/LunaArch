# LunaArch

A modern, AOT-friendly DDD and Clean Architecture framework for .NET 10 and C# 14.

## Overview

LunaArch is a set of reusable libraries that implement Domain-Driven Design (DDD) patterns and Clean Architecture principles. It provides a solid foundation for building enterprise applications with proper separation of concerns, testability, and maintainability.

## Features

- **Clean Architecture** - Strict layer separation with proper dependency direction
- **Domain-Driven Design** - First-class support for Aggregates, Entities, Value Objects, Domain Events
- **CQRS Pattern** - Command/Query separation with a lightweight, AOT-friendly dispatcher
- **Repository Pattern** - Abstracted persistence with Specification pattern support
- **Pipeline Behaviors** - Cross-cutting concerns like logging and transactions
- **AOT Compatible** - Designed for Ahead-of-Time compilation with explicit registrations
- **Multi-tenancy** - Optional add-on for SaaS applications
- **Test Kit** - Fake implementations for easy unit testing

## Getting Started

See [Getting Started Guide](docs/getting-started.md) for installation and setup instructions.

## Documentation

- [Architecture Overview](docs/architecture/overview.md)
- [Layer Responsibilities](docs/architecture/layers.md)
- [Domain Primitives](docs/architecture/domain-primitives.md)
- [CQRS and Messaging](docs/architecture/cqrs.md)
- [Persistence](docs/architecture/persistence.md)
- [Multi-tenancy](docs/architecture/multi-tenancy.md)
- [Testing](docs/architecture/testing.md)

## Package Structure

| Package | Description |
|---------|-------------|
| [`LunaArch.Abstractions`](src/LunaArch.Abstractions/) | Core interfaces and primitives (zero dependencies) |
| [`LunaArch.Domain`](src/LunaArch.Domain/) | Domain layer utilities (Specifications, Guards, Business Rules) |
| [`LunaArch.Application`](src/LunaArch.Application/) | Application layer (Exceptions, Pipeline Behaviors) |
| [`LunaArch.Infrastructure`](src/LunaArch.Infrastructure/) | Infrastructure implementations (EF Core, Dispatcher) |
| [`LunaArch.AspNetCore`](src/LunaArch.AspNetCore/) | ASP.NET Core integration (Middleware, Extensions) |
| [`LunaArch.Infrastructure.MultiTenancy`](src/LunaArch.Infrastructure.MultiTenancy/) | Multi-tenancy add-on |
| [`LunaArch.TestKit`](src/LunaArch.TestKit/) | Testing utilities and fake implementations |

## Requirements

- .NET 10.0
- C# 14
- Entity Framework Core 10.0 (for Infrastructure)

## License

Apache License 2.0
