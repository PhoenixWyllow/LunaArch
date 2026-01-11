using LunaArch.AspNetCore.Middleware;
using Microsoft.AspNetCore.Builder;

namespace LunaArch.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring LunaArch middleware in the ASP.NET Core pipeline.
/// </summary>
public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        /// Adds LunaArch middleware to the application pipeline.
        /// This includes exception handling and correlation ID middleware.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application for chaining.</returns>
        public WebApplication UseLunaArch()
        {
            app.UseLunaArchCorrelationId();
            app.UseLunaArchExceptionHandling();

            return app;
        }

        /// <summary>
        /// Adds only the exception handling middleware.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application for chaining.</returns>
        public WebApplication UseLunaArchExceptionHandling()
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }

        /// <summary>
        /// Adds only the correlation ID middleware.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application for chaining.</returns>
        public WebApplication UseLunaArchCorrelationId()
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }
    }
}
