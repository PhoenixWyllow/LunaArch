using LunaArch.Abstractions.Messaging;

namespace LunaArch.TestKit.Fakes;

/// <summary>
/// Fake implementation of <see cref="IDispatcher"/> for testing.
/// </summary>
public sealed class FakeDispatcher : IDispatcher
{
    private readonly List<object> _dispatched = [];

    /// <summary>
    /// Gets all dispatched commands and queries.
    /// </summary>
    public IReadOnlyList<object> Dispatched => _dispatched.AsReadOnly();

    /// <summary>
    /// Gets or sets the result to return for the next dispatch.
    /// </summary>
    public object? NextResult { get; set; }

    /// <summary>
    /// Gets or sets a factory to generate results based on the request.
    /// </summary>
    public Func<object, object?>? ResultFactory { get; set; }

    /// <inheritdoc />
    public Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        _dispatched.Add(command);
        return Task.FromResult(GetResult<TResult>(command));
    }

    /// <inheritdoc />
    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        _dispatched.Add(query);
        return Task.FromResult(GetResult<TResult>(query));
    }

    /// <summary>
    /// Clears all dispatched items.
    /// </summary>
    public void Clear()
    {
        _dispatched.Clear();
        NextResult = null;
        ResultFactory = null;
    }

    /// <summary>
    /// Gets dispatched items of a specific type.
    /// </summary>
    public IEnumerable<T> GetDispatched<T>() => _dispatched.OfType<T>();

    /// <summary>
    /// Checks if a specific type was dispatched.
    /// </summary>
    public bool WasDispatched<T>() => _dispatched.OfType<T>().Any();

    private TResult GetResult<TResult>(object request)
    {
        if (ResultFactory is not null)
        {
            return (TResult)ResultFactory(request)!;
        }

        if (NextResult is TResult result)
        {
            return result;
        }

        return default!;
    }
}
