namespace LunaArch.Abstractions.Results;

/// <summary>
/// Represents the result of an operation without a value.
/// </summary>
public class Result
{
    private Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    public static Result<T> Success<T>(T value) => Result<T>.CreateSuccess(value);

    /// <summary>
    /// Creates a failure result with a value type.
    /// </summary>
    public static Result<T> Failure<T>(string error) => Result<T>.CreateFailure(error);
}

/// <summary>
/// Represents the result of an operation with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class Result<T>
{
    private Result(T value, bool isSuccess, string? error = null)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets the result value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Internal factory method for success results.
    /// </summary>
    internal static Result<T> CreateSuccess(T value) => new(value, true);

    /// <summary>
    /// Internal factory method for failure results.
    /// </summary>
    internal static Result<T> CreateFailure(string error) => new(default!, false, error);
}
