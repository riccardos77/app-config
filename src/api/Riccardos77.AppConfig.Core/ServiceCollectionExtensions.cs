using Microsoft.Extensions.DependencyInjection;
using Riccardos77.AppConfig.ValueManagers;

namespace Riccardos77.AppConfig
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValueManagers(this IServiceCollection services)
        {
            services.AddSingleton<AppValuesSchemaGenerator>();
            services.AddSingleton<AppValuesInstanceParser>();
            services.AddSingleton<AppValuesInstanceSchemaGenerator>();
            services.AddSingleton<AppValueFileParser>();

            return services;
        }
    }
}
