using System.Data;

namespace LunaArch.Abstractions.Persistence;

/// <summary>
/// Factory for creating database connections.
/// Use this for direct SQL queries (e.g., with Dapper) for optimized read operations.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and opens a new database connection.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An open database connection.</returns>
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
