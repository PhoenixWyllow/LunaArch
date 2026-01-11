using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.AspNetCore.MinimalApi;

/// <summary>
/// Extension methods for registering and mapping endpoint groups.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Registers all endpoint groups from the specified assembly.
    /// </summary>
    /// <typeparam name="TMarker">A type from the assembly containing endpoint groups.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    [RequiresUnreferencedCode("This method uses reflection to scan for IEndpointGroup implementations. For AOT compatibility, use AddEndpointGroup<T>() instead.")]
    public static IServiceCollection AddEndpointGroups<TMarker>(this IServiceCollection services)
    {
        var assembly = typeof(TMarker).Assembly;
        var endpointGroupTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && 
                        t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var type in endpointGroupTypes)
        {
            services.AddSingleton(typeof(IEndpointGroup), type);
        }

        return services;
    }

    /// <summary>
    /// Registers a specific endpoint group (AOT-compatible).
    /// </summary>
    /// <typeparam name="TEndpointGroup">The endpoint group type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddEndpointGroup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEndpointGroup>(
        this IServiceCollection services)
        where TEndpointGroup : class, IEndpointGroup
    {
        services.AddSingleton<IEndpointGroup, TEndpointGroup>();
        return services;
    }

    /// <summary>
    /// Maps all registered endpoint groups.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapEndpointGroups(this IEndpointRouteBuilder app)
    {
        var endpointGroups = app.ServiceProvider.GetServices<IEndpointGroup>();

        foreach (var group in endpointGroups)
        {
            group.MapEndpoints(app);
        }

        return app;
    }

    /// <summary>
    /// Maps a single endpoint group (AOT-compatible, no DI required).
    /// </summary>
    /// <typeparam name="TEndpointGroup">The endpoint group type.</typeparam>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder for chaining.</returns>
    public static IEndpointRouteBuilder MapEndpointGroup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TEndpointGroup>(
        this IEndpointRouteBuilder app)
        where TEndpointGroup : IEndpointGroup, new()
    {
        var group = new TEndpointGroup();
        group.MapEndpoints(app);
        return app;
    }
}
