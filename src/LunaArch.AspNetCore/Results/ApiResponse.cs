using System.Diagnostics.CodeAnalysis;

namespace LunaArch.AspNetCore.Results;

/// <summary>
/// Standard API response wrapper for consistent response formatting.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Factory methods are intentional API design for creating responses")]
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Gets a value indicating whether the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the response data.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the error message if the request failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the error code if the request failed.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the timestamp of the response.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    /// <param name="data">The response data.</param>
    /// <returns>A successful API response.</returns>
    public static ApiResponse<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>
    /// Creates a failed response with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <returns>A failed API response.</returns>
    public static ApiResponse<T> Fail(string error, string? errorCode = null) => new()
    {
        Success = false,
        Error = error,
        ErrorCode = errorCode
    };
}

/// <summary>
/// Non-generic API response for operations that don't return data.
/// </summary>
public sealed class ApiResponse
{
    /// <summary>
    /// Gets a value indicating whether the request was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Gets the error message if the request failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the error code if the request failed.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the timestamp of the response.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <returns>A successful API response.</returns>
    public static ApiResponse Ok() => new() { Success = true };

    /// <summary>
    /// Creates a failed response with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <returns>A failed API response.</returns>
    public static ApiResponse Fail(string error, string? errorCode = null) => new()
    {
        Success = false,
        Error = error,
        ErrorCode = errorCode
    };
}
