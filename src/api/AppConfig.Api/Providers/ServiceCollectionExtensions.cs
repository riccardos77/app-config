using Riccardos77.AppConfig.Abstractions;
using Riccardos77.AppConfig.Api.Infrastructure;

namespace Riccardos77.AppConfig.Api.Providers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureAppDefinitions(this IServiceCollection services, IConfiguration configuration, Action<IAppDefinitionsBuilder> builder)
        {
            var dataAccessProviderFactoryBuilder = new Builder(services);
            builder(dataAccessProviderFactoryBuilder);

            var factory = new DataAccessProviderFactory();

            configuration
                .GetSection("AppCatalog")
                .GetChildren()
                .ToList()
                .ForEach(appSection =>
                {
                    services.Configure<AppDefinitionOptions>(appSection.Key, appSection);

                    factory.ConfigureProvider(appSection.Key, appSection.GetSection("DataAccess").GetFirstChild(), dataAccessProviderFactoryBuilder.Providers);
                });

            services.AddSingleton<IDataAccessProviderFactory>(factory);

            return services;
        }

        private class Builder : IAppDefinitionsBuilder
        {
            public Builder(IServiceCollection services)
            {
                this.Services = services;
            }

            public Dictionary<string, Type> Providers { get; } = new();

            public IServiceCollection Services { get; }

            public IAppDefinitionsBuilder RegisterDataAccessProvider<TProvider>(string providerName)
                where TProvider : IDataAccessProvider, new()
            {
                this.Providers.Add(providerName, typeof(TProvider));

                return this;
            }
        }
    }
}
