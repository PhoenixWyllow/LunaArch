using FastEndpoints;
using LunaArch.Abstractions.Messaging;

namespace LunaArch.AspNetCore.FastEndpoints;

/// <summary>
/// Base class for query endpoints that dispatch queries via the LunaArch dispatcher.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <example>
/// <code>
/// public class GetOrderEndpoint : QueryEndpoint&lt;GetOrderRequest, OrderResponse&gt;
/// {
///     public override void Configure()
///     {
///         Get("/api/orders/{id}");
///         AllowAnonymous();
///     }
///     
///     public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
///     {
///         var query = new GetOrderByIdQuery(req.Id);
///         var result = await SendQueryAsync&lt;GetOrderByIdQuery, OrderDto?&gt;(query, ct);
///         
///         if (result is null)
///         {
///             await SendNotFoundAsync(ct);
///             return;
///         }
///         
///         await SendOkAsync(new OrderResponse(result), ct);
///     }
/// }
/// </code>
/// </example>
public abstract class QueryEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Gets the dispatcher for sending queries.
    /// </summary>
    protected IDispatcher Dispatcher => Resolve<IDispatcher>();

    /// <summary>
    /// Sends a query to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The query result.</returns>
    protected Task<TResult> SendQueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        return Dispatcher.QueryAsync<TQuery, TResult>(query, cancellationToken);
    }
}

/// <summary>
/// Base class for query endpoints without a request body (e.g., GET all).
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class QueryEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<TResponse>
{
    /// <summary>
    /// Gets the dispatcher for sending queries.
    /// </summary>
    protected IDispatcher Dispatcher => Resolve<IDispatcher>();

    /// <summary>
    /// Sends a query to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TQuery">The query type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The query result.</returns>
    protected Task<TResult> SendQueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        return Dispatcher.QueryAsync<TQuery, TResult>(query, cancellationToken);
    }
}
