namespace LunaArch.Application.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// Typically maps to HTTP 404 Not Found.
/// </summary>
public sealed class NotFoundException : ApplicationException
{
    /// <inheritdoc />
    public override string Code => "ENTITY_NOT_FOUND";

    /// <summary>
    /// Gets the name of the entity type that was not found.
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    /// Gets the identifier that was searched for.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    /// <param name="entityName">The name of the entity type.</param>
    /// <param name="entityId">The identifier that was not found.</param>
    public NotFoundException(string entityName, object? entityId)
        : base($"Entity '{entityName}' with identifier '{entityId}' was not found.")
    {
        EntityName = entityName;
        EntityId = entityId;
    }

    /// <summary>
    /// Initializes a new instance with a custom message.
    /// </summary>
    /// <param name="message">The custom error message.</param>
    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
    }

    /// <summary>
    /// Creates a NotFoundException for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entityId">The identifier that was not found.</param>
    /// <returns>A new NotFoundException.</returns>
    public static NotFoundException For<TEntity>(object entityId) =>
        new(typeof(TEntity).Name, entityId);
}
