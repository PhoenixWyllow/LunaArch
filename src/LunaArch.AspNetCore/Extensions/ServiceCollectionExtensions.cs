using LunaArch.Abstractions.Services;
using LunaArch.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.AspNetCore.Extensions;

/// <summary>
/// Extension methods for registering LunaArch ASP.NET Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds LunaArch ASP.NET Core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLunaArchAspNetCore(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();

        return services;
    }
}
