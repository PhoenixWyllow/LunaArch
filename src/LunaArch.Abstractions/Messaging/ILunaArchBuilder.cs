using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Events;

namespace LunaArch.Abstractions.Messaging;

/// <summary>
/// Abstraction for configuring LunaArch handler registrations.
/// Enables Application-layer projects to self-register their own handlers
/// without taking a dependency on Infrastructure.
/// </summary>
/// <example>
/// <code>
/// // In MyApp.Application/DependencyInjection.cs
/// public static class DependencyInjection
/// {
///     public static ILunaArchBuilder AddApplicationHandlers(this ILunaArchBuilder builder)
///     {
///         builder.AddCommandHandler&lt;CreateOrderCommand, OrderId, CreateOrderCommandHandler&gt;();
///         builder.AddQueryHandler&lt;GetOrderQuery, OrderDto, GetOrderQueryHandler&gt;();
///         builder.AddDomainEventHandler&lt;OrderCreatedEvent, SendConfirmationHandler&gt;();
///         builder.AddBehavior&lt;ValidationBehavior&gt;();
///
///         return builder;
///     }
/// }
/// </code>
/// </example>
public interface ILunaArchBuilder
{
    /// <summary>
    /// Registers a command handler.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddCommandHandler<TCommand, TResult, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TCommand : ICommand<TResult>
        where THandler : class, ICommandHandler<TCommand, TResult>;

    /// <summary>
    /// Registers a query handler.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddQueryHandler<TQuery, TResult, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TQuery : IQuery<TResult>
        where THandler : class, IQueryHandler<TQuery, TResult>;

    /// <summary>
    /// Registers a domain event type for AOT-compatible dispatch.
    /// This is required for dispatching events from entity collections using the non-generic overload.
    /// </summary>
    /// <typeparam name="TEvent">The domain event type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddDomainEvent<TEvent>() where TEvent : IDomainEvent;

    /// <summary>
    /// Registers a domain event handler.
    /// This also automatically registers the event type for AOT-compatible dispatch.
    /// </summary>
    /// <typeparam name="TEvent">The domain event type.</typeparam>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddDomainEventHandler<TEvent, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>()
        where TEvent : IDomainEvent
        where THandler : class, IDomainEventHandler<TEvent>;

    /// <summary>
    /// Registers a pipeline behavior. The behavior type must implement
    /// <see cref="IPipelineBehavior{TRequest, TResponse}"/> for at least one pair of type arguments.
    /// </summary>
    /// <typeparam name="TBehavior">The behavior type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddBehavior<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicConstructors)] TBehavior>()
        where TBehavior : class;

    /// <summary>
    /// Registers a pipeline behavior for specific request/response types.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <typeparam name="TBehavior">The behavior implementation type.</typeparam>
    /// <returns>The builder for chaining.</returns>
    ILunaArchBuilder AddBehavior<TRequest, TResponse, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TBehavior>()
        where TBehavior : class, IPipelineBehavior<TRequest, TResponse>;
}
