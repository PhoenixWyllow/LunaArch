using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.Infrastructure.Messaging;

/// <summary>
/// Builder for configuring LunaArch services including handlers and domain events.
/// Provides a fluent API for explicit registration (fully AOT-compatible).
/// </summary>
/// <example>
/// <code>
/// services.AddLunaArch(builder =>
/// {
///     // Register command handlers
///     builder.AddCommandHandler&lt;CreateOrderCommand, OrderId, CreateOrderCommandHandler&gt;();
///     
///     // Register query handlers
///     builder.AddQueryHandler&lt;GetOrderQuery, OrderDto, GetOrderQueryHandler&gt;();
///     
///     // Register domain events (required for non-generic dispatch)
///     builder.AddDomainEvent&lt;OrderCreatedEvent&gt;();
///     builder.AddDomainEvent&lt;OrderCompletedEvent&gt;();
///     
///     // Register domain event handlers
///     builder.AddDomainEventHandler&lt;OrderCreatedEvent, SendOrderConfirmationHandler&gt;();
///     
///     // Register pipeline behaviors
///     builder.AddBehavior&lt;ValidationBehavior&gt;();
/// });
/// </code>
/// </example>
public sealed class LunaArchBuilder
{
    private readonly IServiceCollection _services;
    private readonly DomainEventRegistry _eventRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="LunaArchBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="eventRegistry">The domain event registry.</param>
    public LunaArchBuilder(IServiceCollection services, DomainEventRegistry eventRegistry)
    {
        _services = services;
        _eventRegistry = eventRegistry;
    }

    /// <summary>
    /// Registers a command handler.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    public LunaArchBuilder AddCommandHandler<TCommand, TResult, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TCommand : ICommand<TResult>
        where THandler : class, ICommandHandler<TCommand, TResult>
    {
        _services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>();
        return this;
    }

    /// <summary>
    /// Registers a query handler.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    public LunaArchBuilder AddQueryHandler<TQuery, TResult, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TQuery : IQuery<TResult>
        where THandler : class, IQueryHandler<TQuery, TResult>
    {
        _services.AddScoped<IQueryHandler<TQuery, TResult>, THandler>();
        return this;
    }

    /// <summary>
    /// Registers a domain event type for AOT-compatible dispatch.
    /// This is required for dispatching events from entity collections using the non-generic overload.
    /// </summary>
    /// <typeparam name="TEvent">The domain event type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    public LunaArchBuilder AddDomainEvent<TEvent>() where TEvent : IDomainEvent
    {
        _eventRegistry.Register<TEvent>();
        return this;
    }

    /// <summary>
    /// Registers a domain event handler.
    /// This also automatically registers the event type for AOT-compatible dispatch.
    /// </summary>
    /// <typeparam name="TEvent">The domain event type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    public LunaArchBuilder AddDomainEventHandler<TEvent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TEvent : IDomainEvent
        where THandler : class, IDomainEventHandler<TEvent>
    {
        // Register the event type for AOT-compatible dispatch
        _eventRegistry.Register<TEvent>();

        // Register the handler
        _services.AddScoped<IDomainEventHandler<TEvent>, THandler>();
        return this;
    }

    /// <summary>
    /// Registers a pipeline behavior.
    /// </summary>
    /// <typeparam name="TBehavior">The behavior type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    [SuppressMessage("Trimming", "IL2090:Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method", Justification = "Behavior types are explicitly registered by the consumer")]
    [SuppressMessage("Trimming", "IL2087:'implementationType' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method", Justification = "Behavior types are explicitly registered by the consumer")]
    public LunaArchBuilder AddBehavior<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] TBehavior>()
        where TBehavior : class
    {
        var behaviorType = typeof(TBehavior);
        var pipelineInterface = behaviorType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        if (pipelineInterface is not null)
        {
            _services.AddScoped(pipelineInterface, behaviorType);
        }

        return this;
    }

    /// <summary>
    /// Registers a pipeline behavior for specific request/response types.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="TBehavior">The behavior implementation type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    public LunaArchBuilder AddBehavior<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBehavior>()
        where TBehavior : class, IPipelineBehavior<TRequest, TResponse>
    {
        _services.AddScoped<IPipelineBehavior<TRequest, TResponse>, TBehavior>();
        return this;
    }
}
