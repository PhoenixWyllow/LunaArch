using LunaArch.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace LunaArch.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs command and query execution.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed partial class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        PipelineDelegate<TResponse> continuation,
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;

        LogHandling(logger, requestName, request);

        var response = await continuation(cancellationToken);

        LogHandled(logger, requestName);

        return response;
    }

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Handling {RequestName}: {Request}")]
    private static partial void LogHandling(ILogger logger, string requestName, TRequest request);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Handled {RequestName} successfully")]
    private static partial void LogHandled(ILogger logger, string requestName);
}
