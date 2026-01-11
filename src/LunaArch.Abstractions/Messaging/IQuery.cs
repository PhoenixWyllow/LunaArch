namespace LunaArch.Abstractions.Messaging;

/// <summary>
/// Marker interface for queries that return a result.
/// Queries represent a request for data without changing state.
/// </summary>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQuery<TResult>;
