using Riccardos77.AppConfig.Abstractions;
using System.Collections.Concurrent;

namespace Riccardos77.AppConfig.Api.Providers;

public class DataAccessProviderFactory : IDataAccessProviderFactory
{
    private readonly ConcurrentDictionary<string, IDataAccessProvider> providers = new();

    public IDataAccessProvider GetProvider(string appName)
    {
        return this.providers[appName];
    }

    public void ConfigureProvider(string appName, IConfigurationSection providerSection, Dictionary<string, Type> providerTypes)
    {
        var providerType = providerTypes[providerSection.Key];

        if (Activator.CreateInstance(providerType) is IDataAccessProvider providerInstance)
        {
            providerInstance.AppName = appName;
            providerSection.Bind(providerInstance.Options);

            this.providers[appName] = providerInstance;
        }
    }
}
