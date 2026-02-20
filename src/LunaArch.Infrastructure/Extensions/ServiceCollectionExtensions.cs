using System.Diagnostics.CodeAnalysis;
using LunaArch.Abstractions.Events;
using LunaArch.Abstractions.Messaging;
using LunaArch.Abstractions.Persistence;
using LunaArch.Abstractions.Services;
using LunaArch.Infrastructure.Messaging;
using LunaArch.Infrastructure.Persistence;
using LunaArch.Infrastructure.Persistence.Interceptors;
using LunaArch.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LunaArch.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering LunaArch infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds LunaArch infrastructure services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureLunaArch">Optional action to configure LunaArch services.</param>
        /// <returns>The service collection for chaining.</returns>
        public IServiceCollection AddLunaArch(
            Action<LunaArchBuilder>? configureLunaArch = null)
        {
            // Create and register the domain event registry
            var eventRegistry = new DomainEventRegistry();
            services.AddSingleton(eventRegistry);

            // Register core services
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Register interceptors
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();

            // Configure LunaArch
            if (configureLunaArch is not null)
            {
                var builder = new LunaArchBuilder(services, eventRegistry);
                configureLunaArch(builder);
            }

            // Freeze the registry after configuration
            eventRegistry.Freeze();

            return services;
        }

        /// <summary>
        /// Adds LunaArch infrastructure with a specific DbContext.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configureDbContext">Action to configure the DbContext.</param>
        /// <param name="configureLunaArch">Optional action to configure LunaArch services.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <remarks>
        /// EF Core is not fully AOT/trimming compatible. See https://aka.ms/efcore-docs-trimming
        /// </remarks>
        [SuppressMessage("Trimming", "IL2091:Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method", Justification = "EF Core is not fully AOT compatible")]
        public IServiceCollection AddLunaArch<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TContext>(
            Action<DbContextOptionsBuilder> configureDbContext,
            Action<LunaArchBuilder>? configureLunaArch = null)
            where TContext : DbContextBase
        {
            services.AddLunaArch(configureLunaArch);

            services.AddDbContext<TContext>((sp, options) =>
            {
                configureDbContext(options);

                // Add interceptors
                options.AddInterceptors(
                    sp.GetRequiredService<AuditableEntityInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>());
            });

            // Register DbContext base type for repository resolution
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<TContext>());

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();

            return services;
        }
    }
}
