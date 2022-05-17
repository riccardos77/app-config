using Riccardos77.AppConfig.DataProviders.Abstractions;
using System.Collections.Concurrent;

namespace Riccardos77.AppConfig.DataProviders;

public class DataProviderFactory : IDataProviderFactory
{
    private readonly ConcurrentDictionary<string, IDataProvider> providers = new();

    public IDataProvider GetProvider(string appName)
    {
        return this.providers[appName];
    }

    public void AddProvider(IDataProvider providerInstance)
    {
        this.providers[providerInstance.AppName] = providerInstance;
    }
}
