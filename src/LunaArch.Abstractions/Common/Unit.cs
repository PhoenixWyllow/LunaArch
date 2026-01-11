// Justification: Unit is a singleton-like type where all instances are semantically identical.
// The operator parameters exist for the required signature but are intentionally unused.
// The explicit default initialization documents intent.
#pragma warning disable CA1805 // Do not initialize unnecessarily - explicit default documents intent
#pragma warning disable IDE0060 // Remove unused parameter - required for operator signature

namespace LunaArch.Abstractions.Common;

/// <summary>
/// Represents a void return type for commands that don't return a value.
/// Use this as the return type for commands that modify state without returning data.
/// </summary>
/// <example>
/// <code>
/// public sealed record DeleteOrderCommand(Guid OrderId) : ICommand&lt;Unit&gt;;
/// </code>
/// </example>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    /// <summary>
    /// Gets the single value of the Unit type.
    /// </summary>
    public static readonly Unit Value = default;

    /// <inheritdoc />
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <summary>
    /// Equality operator. All Unit instances are equal.
    /// </summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Inequality operator. No Unit instances are unequal.
    /// </summary>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Less-than operator. No Unit is less than another.
    /// </summary>
    public static bool operator <(Unit left, Unit right) => false;

    /// <summary>
    /// Greater-than operator. No Unit is greater than another.
    /// </summary>
    public static bool operator >(Unit left, Unit right) => false;

    /// <summary>
    /// Less-than-or-equal operator. All Units are equal.
    /// </summary>
    public static bool operator <=(Unit left, Unit right) => true;

    /// <summary>
    /// Greater-than-or-equal operator. All Units are equal.
    /// </summary>
    public static bool operator >=(Unit left, Unit right) => true;
}
