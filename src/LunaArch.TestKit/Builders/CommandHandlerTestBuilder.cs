using LunaArch.Abstractions.Messaging;

namespace LunaArch.TestKit.Builders;

/// <summary>
/// Builder for creating command handler test scenarios.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResult">The result type.</typeparam>
/// <typeparam name="THandler">The handler type.</typeparam>
public class CommandHandlerTestBuilder<TCommand, TResult, THandler>
    where TCommand : ICommand<TResult>
    where THandler : ICommandHandler<TCommand, TResult>
{
    private THandler? _handler;
    private TCommand? _command;
    private readonly List<Action> _setupActions = [];

    /// <summary>
    /// Sets the handler to test.
    /// </summary>
    public CommandHandlerTestBuilder<TCommand, TResult, THandler> WithHandler(THandler handler)
    {
        _handler = handler;
        return this;
    }

    /// <summary>
    /// Sets the command to execute.
    /// </summary>
    public CommandHandlerTestBuilder<TCommand, TResult, THandler> WithCommand(TCommand command)
    {
        _command = command;
        return this;
    }

    /// <summary>
    /// Adds a setup action to run before execution.
    /// </summary>
    public CommandHandlerTestBuilder<TCommand, TResult, THandler> WithSetup(Action action)
    {
        _setupActions.Add(action);
        return this;
    }

    /// <summary>
    /// Executes the command and returns the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Handler must be set before execution.");
        }

        if (_command is null)
        {
            throw new InvalidOperationException("Command must be set before execution.");
        }

        foreach (var action in _setupActions)
        {
            action();
        }

        return await _handler.HandleAsync(_command, cancellationToken);
    }

    /// <summary>
    /// Executes the command expecting an exception.
    /// </summary>
    public async Task<TException> ExecuteExpectingExceptionAsync<TException>(
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (_handler is null)
        {
            throw new InvalidOperationException("Handler must be set before execution.");
        }

        if (_command is null)
        {
            throw new InvalidOperationException("Command must be set before execution.");
        }

        foreach (var action in _setupActions)
        {
            action();
        }

        try
        {
            await _handler.HandleAsync(_command, cancellationToken);
            throw new InvalidOperationException($"Expected exception of type {typeof(TException).Name} but none was thrown.");
        }
        catch (TException ex)
        {
            return ex;
        }
    }
}
