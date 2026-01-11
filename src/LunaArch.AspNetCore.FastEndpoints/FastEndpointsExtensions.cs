using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.AspNetCore.FastEndpoints;

/// <summary>
/// Extension methods for configuring FastEndpoints with LunaArch.
/// </summary>
public static class FastEndpointsExtensions
{
    /// <summary>
    /// Adds FastEndpoints to the service collection with LunaArch defaults.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureDiscovery">Optional action to configure endpoint discovery.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // Basic usage
    /// builder.Services.AddLunaArchFastEndpoints();
    /// 
    /// // With custom discovery configuration
    /// builder.Services.AddLunaArchFastEndpoints(options =>
    /// {
    ///     options.Filter = t => t.Namespace?.Contains("Endpoints") == true;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddLunaArchFastEndpoints(
        this IServiceCollection services,
        Action<EndpointDiscoveryOptions>? configureDiscovery = null)
    {
        services.AddFastEndpoints(configureDiscovery);
        return services;
    }
}
