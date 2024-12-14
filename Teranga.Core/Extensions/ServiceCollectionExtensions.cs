
using Microsoft.Extensions.DependencyInjection;
using Teranga.Core.Services;

namespace Teranga.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTerangaCore(this IServiceCollection services)
        {
            services.AddScoped<ITerangaService, TerangaService>();
            return services;
        }
    }
}
