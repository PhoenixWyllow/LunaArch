using FastEndpoints;
using LunaArch.Abstractions.Messaging;

namespace LunaArch.AspNetCore.FastEndpoints;

/// <summary>
/// Base class for command endpoints that dispatch commands via the LunaArch dispatcher.
/// </summary>
/// <typeparam name="TRequest">The request/command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <example>
/// <code>
/// public class CreateOrderEndpoint : CommandEndpoint&lt;CreateOrderRequest, OrderResponse&gt;
/// {
///     public override void Configure()
///     {
///         Post("/api/orders");
///         AllowAnonymous();
///     }
///     
///     public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
///     {
///         var command = new CreateOrderCommand(req.CustomerId, req.Items);
///         var result = await SendCommandAsync&lt;CreateOrderCommand, Guid&gt;(command, ct);
///         await SendCreatedAtAsync&lt;GetOrderEndpoint&gt;(new { id = result }, new OrderResponse(result), cancellation: ct);
///     }
/// }
/// </code>
/// </example>
public abstract class CommandEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Gets the dispatcher for sending commands and queries.
    /// </summary>
    protected IDispatcher Dispatcher => Resolve<IDispatcher>();

    /// <summary>
    /// Sends a command to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The command result.</returns>
    protected Task<TResult> SendCommandAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : Abstractions.Messaging.ICommand<TResult>
    {
        return Dispatcher.SendAsync<TCommand, TResult>(command, cancellationToken);
    }
}

/// <summary>
/// Base class for command endpoints without a response body (returns 204 No Content).
/// </summary>
/// <typeparam name="TRequest">The request/command type.</typeparam>
public abstract class CommandEndpointWithoutResponse<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    /// <summary>
    /// Gets the dispatcher for sending commands.
    /// </summary>
    protected IDispatcher Dispatcher => Resolve<IDispatcher>();

    /// <summary>
    /// Sends a command to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TCommand">The command type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The command result.</returns>
    protected Task<TResult> SendCommandAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : Abstractions.Messaging.ICommand<TResult>
    {
        return Dispatcher.SendAsync<TCommand, TResult>(command, cancellationToken);
    }
}
