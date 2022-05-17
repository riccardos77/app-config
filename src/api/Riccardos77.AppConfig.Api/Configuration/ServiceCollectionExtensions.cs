using Riccardos77.AppConfig.Configuration.Abstractions;
using Riccardos77.AppConfig.DataProviders;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAppDefinitions(this IServiceCollection services, IConfiguration configuration, Action<IAppDefinitionsBuilder> builder)
    {
        var appDefinitionBuilder = new Builder(services);
        builder(appDefinitionBuilder);

        var factory = new DataProviderFactory();

        configuration
            .GetSection("AppCatalog")
            .GetChildren()
            .ToList()
            .ForEach(appSection =>
            {
                services.Configure<AppDefinitionOptions>(appSection.Key, appSection);

                var instance = ConfigureProvider(appSection.Key, appSection.GetSection("DataProvider").GetFirstChild(), appDefinitionBuilder.Providers);
                factory.AddProvider(instance);
            });

        services.AddSingleton<IDataProviderFactory>(factory);

        return services;

        static IDataProvider ConfigureProvider(string appName, IConfigurationSection providerSection, Dictionary<string, Type> providerTypes)
        {
            var providerType = providerTypes[providerSection.Key];

            if (Activator.CreateInstance(providerType) is IDataProvider providerInstance)
            {
                providerInstance.AppName = appName;
                providerSection.Bind(providerInstance.Options);

                return providerInstance;
            }

            throw new InvalidOperationException();
        }
    }

    internal static IConfigurationSection GetFirstChild(this IConfigurationSection configurationSection)
    {
        return configurationSection.GetChildren().First();
    }

    private class Builder : IAppDefinitionsBuilder
    {
        public Builder(IServiceCollection services)
        {
            this.Services = services;
        }

        public Dictionary<string, Type> Providers { get; } = new();

        public IServiceCollection Services { get; }

        public IAppDefinitionsBuilder RegisterDataProvider<TProvider>(string providerName)
            where TProvider : IDataProvider, new()
        {
            this.Providers.Add(providerName, typeof(TProvider));

            return this;
        }
    }
}
