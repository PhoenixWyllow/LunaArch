namespace LunaArch.Application.Exceptions;

/// <summary>
/// Base exception for application layer errors.
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Gets the error code for this exception.
    /// </summary>
    public abstract string Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    protected ApplicationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
