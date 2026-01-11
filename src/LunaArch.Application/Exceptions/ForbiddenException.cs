namespace LunaArch.Application.Exceptions;

/// <summary>
/// Exception thrown when the user is not authorized to perform an operation.
/// Typically maps to HTTP 403 Forbidden.
/// </summary>
public sealed class ForbiddenException : ApplicationException
{
    /// <inheritdoc />
    public override string Code => "FORBIDDEN";

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message)
    {
    }
}
