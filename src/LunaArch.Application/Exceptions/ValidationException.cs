namespace LunaArch.Application.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// Typically maps to HTTP 400 Bad Request.
/// </summary>
public sealed class ValidationException : ApplicationException
{
    /// <inheritdoc />
    public override string Code => "VALIDATION_ERROR";

    /// <summary>
    /// Gets the validation errors grouped by property name.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance with a dictionary of errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance with a single error.
    /// </summary>
    /// <param name="propertyName">The property that failed validation.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }

    /// <summary>
    /// Initializes a new instance with a list of validation failures.
    /// </summary>
    /// <param name="failures">The validation failures.</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray());
    }
}

/// <summary>
/// Represents a single validation failure.
/// </summary>
/// <param name="PropertyName">The name of the property that failed validation.</param>
/// <param name="ErrorMessage">The error message describing the failure.</param>
public sealed record ValidationFailure(string PropertyName, string ErrorMessage);
