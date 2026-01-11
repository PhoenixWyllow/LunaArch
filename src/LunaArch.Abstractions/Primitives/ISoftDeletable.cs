namespace LunaArch.Abstractions.Primitives;

/// <summary>
/// Interface for entities that support soft deletion.
/// Soft-deleted entities are marked as deleted but remain in the database.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets a value indicating whether this entity has been soft deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the date and time when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user who deleted the entity.
    /// </summary>
    string? DeletedBy { get; }

    /// <summary>
    /// Marks the entity as soft deleted.
    /// </summary>
    /// <param name="deletedAt">The deletion timestamp.</param>
    /// <param name="deletedBy">The identifier of the user performing the deletion.</param>
    void SoftDelete(DateTimeOffset deletedAt, string? deletedBy);

    /// <summary>
    /// Restores a soft-deleted entity.
    /// </summary>
    void Restore();
}
