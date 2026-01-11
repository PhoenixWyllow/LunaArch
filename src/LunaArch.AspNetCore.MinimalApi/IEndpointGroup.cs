using Microsoft.AspNetCore.Routing;

namespace LunaArch.AspNetCore.MinimalApi;

/// <summary>
/// Interface for defining a group of related endpoints.
/// Implement this interface to organize your Minimal API endpoints into logical groups.
/// </summary>
/// <example>
/// <code>
/// public class OrderEndpoints : IEndpointGroup
/// {
///     public void MapEndpoints(IEndpointRouteBuilder app)
///     {
///         var group = app.MapGroup("/api/orders").WithTags("Orders");
///         
///         group.MapGet("/", GetOrders);
///         group.MapGet("/{id:guid}", GetOrderById);
///         group.MapPost("/", CreateOrder);
///     }
/// }
/// </code>
/// </example>
public interface IEndpointGroup
{
    /// <summary>
    /// Maps the endpoints for this group.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    void MapEndpoints(IEndpointRouteBuilder app);
}
