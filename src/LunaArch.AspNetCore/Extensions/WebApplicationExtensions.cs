using LunaArch.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace LunaArch.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring LunaArch middleware in the ASP.NET Core pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds LunaArch middleware to the application pipeline.
    /// This includes exception handling and correlation ID middleware.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseLunaArch(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        return app;
    }

    /// <summary>
    /// Adds only the exception handling middleware.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseLunaArchExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    /// <summary>
    /// Adds only the correlation ID middleware.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication UseLunaArchCorrelationId(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}
