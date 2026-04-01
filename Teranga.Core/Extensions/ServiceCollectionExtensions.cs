using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Teranga.Core.Services;

namespace Teranga.Core.Extensions
{
    /// <summary>
    /// The service collection extensions of the application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the Teranga core services.
        /// </summary>
        public static IServiceCollection AddTerangaCore(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.TryAdd(ServiceDescriptor.Describe(typeof(ITerangaService), typeof(TerangaService), serviceLifetime));
            services.TryAdd(ServiceDescriptor.Describe(typeof(IAnsdRegionalStatisticsService), typeof(AnsdRegionalStatisticsService), serviceLifetime));
            return services;
        }
    }
}
