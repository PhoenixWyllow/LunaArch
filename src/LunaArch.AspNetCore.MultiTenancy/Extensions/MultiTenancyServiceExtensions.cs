using System.Diagnostics.CodeAnalysis;
using LunaArch.AspNetCore.MultiTenancy.Abstractions;
using LunaArch.AspNetCore.MultiTenancy.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.AspNetCore.MultiTenancy.Extensions;

/// <summary>
/// Extension methods for configuring multi-tenancy.
/// </summary>
public static class MultiTenancyServiceExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds multi-tenancy services to the service collection.
        /// </summary>
        /// <typeparam name="TTenantStore">The tenant store implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddLunaArchMultiTenancy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTenantStore>()
            where TTenantStore : class, ITenantStore
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver, HeaderTenantResolver>();
            services.AddScoped<ITenantStore, TTenantStore>();

            return services;
        }

        /// <summary>
        /// Adds multi-tenancy services with a custom tenant resolver.
        /// </summary>
        /// <typeparam name="TTenantStore">The tenant store implementation type.</typeparam>
        /// <typeparam name="TTenantResolver">The tenant resolver implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddLunaArchMultiTenancy<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTenantStore,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTenantResolver>(
            )
            where TTenantStore : class, ITenantStore
            where TTenantResolver : class, ITenantResolver
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantResolver, TTenantResolver>();
            services.AddScoped<ITenantStore, TTenantStore>();

            return services;
        }
    }
}
