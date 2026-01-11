using LunaArch.AspNetCore.MultiTenancy.Middleware;
using Microsoft.AspNetCore.Builder;

namespace LunaArch.AspNetCore.MultiTenancy.Extensions;

public static class MultiTenancyApplicationExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        /// Adds the tenant resolution middleware to the application pipeline.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application for chaining.</returns>
        public WebApplication UseLunaArchMultiTenancy()
        {
            app.UseMiddleware<TenantResolutionMiddleware>();
            return app;
        }
    }
}
