using Microsoft.Extensions.Options;
using Riccardos77.AppConfig.Api.Configuration;
using Riccardos77.AppConfig.DataProviders.Abstractions;
using System.Collections.Concurrent;

namespace Riccardos77.AppConfig.Api.Infrastructure;

public class DataProviderFactory : IDataProviderFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly ConcurrentDictionary<string, IDataProvider> providers = new();
    private readonly IOptionsMonitor<AppDefinitionOptions> appDefinitionsOptions;

    public DataProviderFactory(
        IServiceProvider serviceProvider,
        IOptionsMonitor<AppDefinitionOptions> appDefinitionsOptions)
    {
        this.serviceProvider = serviceProvider;
        this.appDefinitionsOptions = appDefinitionsOptions;
    }

    public IDataProvider GetProvider(string appName)
    {
        return this.providers.GetOrAdd(appName, a =>
        {
            var appDefinition = this.appDefinitionsOptions.Get(appName);
            var providerType = appDefinition.ProviderType ?? throw new InvalidOperationException("Invalid Service Provider Type");

            if (ActivatorUtilities.GetServiceOrCreateInstance(this.serviceProvider, providerType) is IDataProvider provider)
            {
                provider.SetOptions(appDefinition.ProviderOptions);
                provider.AppName = appName;
                return provider;
            }

            throw new InvalidOperationException("Invalid Service Provider Type");
        });
    }
}
