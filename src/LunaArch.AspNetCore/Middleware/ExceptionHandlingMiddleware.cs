using System.Text.Json;
using LunaArch.Application.Exceptions;
using LunaArch.AspNetCore.Serialization;
using LunaArch.Domain.Rules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LunaArch.AspNetCore.Middleware;

/// <summary>
/// Middleware that handles exceptions and converts them to appropriate HTTP responses.
/// Uses source-generated JSON serialization for AOT compatibility.
/// </summary>
public sealed partial class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            NotFoundException notFound => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    notFound.Message,
                    notFound.Code)),

            ValidationException validation => (
                StatusCodes.Status400BadRequest,
                CreateValidationProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    validation.Message,
                    validation.Code,
                    validation.Errors)),

            ConflictException conflict => (
                StatusCodes.Status409Conflict,
                CreateProblemDetails(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    conflict.Message,
                    conflict.Code)),

            ForbiddenException forbidden => (
                StatusCodes.Status403Forbidden,
                CreateProblemDetails(
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    forbidden.Message,
                    forbidden.Code)),

            BusinessRuleValidationException businessRule => (
                StatusCodes.Status422UnprocessableEntity,
                CreateProblemDetails(
                    StatusCodes.Status422UnprocessableEntity,
                    "Business Rule Violation",
                    businessRule.Message,
                    businessRule.Code)),

            _ => (
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred.",
                    "INTERNAL_ERROR"))
        };

        // Log based on severity
        if (statusCode >= 500)
        {
            LogUnhandledException(logger, exception.Message, exception);
        }
        else
        {
            LogRequestFailed(logger, statusCode, exception.Message);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        // Use source-generated serialization for AOT compatibility
        if (problemDetails is ValidationProblemDetails validationProblem)
        {
            await context.Response.WriteAsJsonAsync(
                validationProblem,
                LunaArchJsonContext.Default.ValidationProblemDetails);
        }
        else
        {
            await context.Response.WriteAsJsonAsync(
                problemDetails,
                LunaArchJsonContext.Default.ProblemDetails);
        }
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode,
        string title,
        string detail,
        string errorCode)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Extensions = { ["errorCode"] = errorCode }
        };
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        int statusCode,
        string title,
        string detail,
        string errorCode,
        IReadOnlyDictionary<string, string[]> errors)
    {
        var problemDetails = new ValidationProblemDetails(
            errors.ToDictionary(x => x.Key, x => x.Value))
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        problemDetails.Extensions["errorCode"] = errorCode;

        return problemDetails;
    }

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Unhandled exception occurred: {Message}")]
    private static partial void LogUnhandledException(ILogger logger, string message, Exception exception);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Request failed with status {StatusCode}: {Message}")]
    private static partial void LogRequestFailed(ILogger logger, int statusCode, string message);
}
