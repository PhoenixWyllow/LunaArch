namespace LunaArch.Abstractions.Messaging;

/// <summary>
/// Interface for dispatching commands and queries to their handlers.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Sends a command to its handler and returns the result.
    /// This overload preserves the command type for AOT-compatible handler resolution.
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task containing the result of the command.</returns>
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>;

    /// <summary>
    /// Sends a query to its handler and returns the result.
    /// This overload preserves the query type for AOT-compatible handler resolution.
    /// </summary>
    /// <typeparam name="TQuery">The type of query.</typeparam>
    /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task containing the result of the query.</returns>
    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
