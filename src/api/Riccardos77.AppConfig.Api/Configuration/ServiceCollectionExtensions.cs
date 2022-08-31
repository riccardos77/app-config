using Riccardos77.AppConfig.Api.Infrastructure;
using Riccardos77.AppConfig.Configuration.Abstractions;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAppDefinitions(this IServiceCollection services, IConfiguration configuration, Action<IAppDefinitionsBuilder> builder)
    {
        var appDefinitionBuilder = new Builder(services);
        builder(appDefinitionBuilder);

        configuration
            .GetSection("AppCatalog")
            .GetChildren()
            .ToList()
            .ForEach(appSection =>
            {
                var appDefinition = new AppDefinitionOptions();
                services.AddOptions<AppDefinitionOptions>(appSection.Key)
                    .Bind(appSection)
                    .Configure(appDef =>
                    {
                        var providerSection = appSection.GetSection("DataProvider").GetChildren().First();
                        var (providerType, optionsType) = appDefinitionBuilder.Providers[providerSection.Key];
                        appDef.ProviderType = providerType;
                        appDef.ProviderOptions = Activator.CreateInstance(optionsType);
                        providerSection.Bind(appDef.ProviderOptions);
                    });
            });

        services.AddSingleton<IDataProviderFactory, DataProviderFactory>();

        return services;
    }

    private class Builder : IAppDefinitionsBuilder
    {
        public Builder(IServiceCollection services)
        {
            this.Services = services;
        }

        public Dictionary<string, (Type Provider, Type Options)> Providers { get; } = new();

        public IServiceCollection Services { get; }

        public IAppDefinitionsBuilder RegisterDataProvider<TProvider, TProviderOptions>(string providerName)
            where TProvider : IDataProvider
            where TProviderOptions : class, new()
        {
            this.Providers.Add(providerName, (typeof(TProvider), typeof(TProviderOptions)));

            return this;
        }
    }
}
