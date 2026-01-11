namespace LunaArch.Application.Exceptions;

/// <summary>
/// Exception thrown when an operation conflicts with the current state.
/// Typically maps to HTTP 409 Conflict.
/// </summary>
public sealed class ConflictException : ApplicationException
{
    /// <inheritdoc />
    public override string Code => "CONFLICT";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConflictException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a ConflictException for a duplicate entity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="identifier">The duplicate identifier.</param>
    /// <returns>A new ConflictException.</returns>
    public static ConflictException DuplicateEntity<TEntity>(string identifier) =>
        new($"Entity '{typeof(TEntity).Name}' with identifier '{identifier}' already exists.");
}
