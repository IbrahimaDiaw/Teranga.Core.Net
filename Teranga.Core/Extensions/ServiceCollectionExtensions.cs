
using Microsoft.Extensions.DependencyInjection;
using Teranga.Core.Services;

namespace Teranga.Core.Extensions
{
    /// <summary>
    /// The service collection extensions of the application
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the Teranga core services
        /// <param name="services"></param>
        /// <returns></returns>
        /// </summary>
        public static IServiceCollection AddTerangaCore(this IServiceCollection services)
        {
            services.AddScoped<ITerangaService, TerangaService>();
            return services;
        }
    }
}
