namespace LunaArch.Abstractions.Messaging;

/// <summary>
/// Delegate for the next step in the pipeline.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <param name="cancellationToken">A cancellation token.</param>
/// <returns>A task containing the response.</returns>
// Justification: PipelineDelegate is a delegate type - the suffix accurately describes what it is.
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public delegate Task<TResponse> PipelineDelegate<TResponse>(CancellationToken cancellationToken);
#pragma warning restore CA1711

/// <summary>
/// Interface for implementing pipeline behaviors that wrap command/query handling.
/// Behaviors can be used for cross-cutting concerns like validation, logging, and transactions.
/// </summary>
/// <typeparam name="TRequest">The type of request being handled.</typeparam>
/// <typeparam name="TResponse">The type of response returned.</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
{
    /// <summary>
    /// Handles the request, optionally invoking the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="continuation">The delegate to invoke the next handler in the pipeline.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task containing the response.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        PipelineDelegate<TResponse> continuation,
        CancellationToken cancellationToken = default);
}
