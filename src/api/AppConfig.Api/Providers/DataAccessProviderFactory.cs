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

    public void RegisterProvider(string appName, IDataAccessProvider provider)
    {
        this.providers[appName] = provider;
    }
}
