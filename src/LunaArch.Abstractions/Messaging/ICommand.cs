namespace LunaArch.Abstractions.Messaging;

/// <summary>
/// Marker interface for commands that return a result.
/// Commands represent an intent to change the state of the system.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommand<TResult>;
