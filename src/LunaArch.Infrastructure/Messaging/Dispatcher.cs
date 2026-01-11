using LunaArch.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.Infrastructure.Messaging;

/// <summary>
/// Lightweight dispatcher implementation that routes commands and queries to their handlers.
/// This implementation is fully AOT-compatible using strongly-typed handler resolution.
/// </summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    /// <inheritdoc />
    public async Task<TResult> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);

        var handler = serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()
            ?? throw new InvalidOperationException(
                $"No handler registered for command type '{typeof(TCommand).Name}'.");

        return await handler.HandleAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TResult> QueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(query);

        var handler = serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()
            ?? throw new InvalidOperationException(
                $"No handler registered for query type '{typeof(TQuery).Name}'.");

        return await handler.HandleAsync(query, cancellationToken);
    }
}
