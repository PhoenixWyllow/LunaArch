using LunaArch.Abstractions.Messaging;

namespace LunaArch.TestKit.Builders;

/// <summary>
/// Builder for creating query handler test scenarios.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
/// <typeparam name="THandler">The handler type.</typeparam>
public class QueryHandlerTestBuilder<TQuery, TResult, THandler>
    where TQuery : IQuery<TResult>
    where THandler : IQueryHandler<TQuery, TResult>
{
    private THandler? _handler;
    private TQuery? _query;
    private readonly List<Action> _setupActions = [];

    /// <summary>
    /// Sets the handler to test.
    /// </summary>
    public QueryHandlerTestBuilder<TQuery, TResult, THandler> WithHandler(THandler handler)
    {
        _handler = handler;
        return this;
    }

    /// <summary>
    /// Sets the query to execute.
    /// </summary>
    public QueryHandlerTestBuilder<TQuery, TResult, THandler> WithQuery(TQuery query)
    {
        _query = query;
        return this;
    }

    /// <summary>
    /// Adds a setup action to run before execution.
    /// </summary>
    public QueryHandlerTestBuilder<TQuery, TResult, THandler> WithSetup(Action action)
    {
        _setupActions.Add(action);
        return this;
    }

    /// <summary>
    /// Executes the query and returns the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Handler must be set before execution.");
        }

        if (_query is null)
        {
            throw new InvalidOperationException("Query must be set before execution.");
        }

        foreach (var action in _setupActions)
        {
            action();
        }

        return await _handler.HandleAsync(_query, cancellationToken);
    }

    /// <summary>
    /// Executes the query expecting an exception.
    /// </summary>
    public async Task<TException> ExecuteExpectingExceptionAsync<TException>(
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Handler must be set before execution.");
        }

        if (_query is null)
        {
            throw new InvalidOperationException("Query must be set before execution.");
        }

        foreach (var action in _setupActions)
        {
            action();
        }

        try
        {
            await _handler.HandleAsync(_query, cancellationToken);
            throw new InvalidOperationException($"Expected exception of type {typeof(TException).Name} but none was thrown.");
        }
        catch (TException ex)
        {
            return ex;
        }
    }
}
