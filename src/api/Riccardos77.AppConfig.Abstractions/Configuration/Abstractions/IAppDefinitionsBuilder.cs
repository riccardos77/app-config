using Microsoft.Extensions.DependencyInjection;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.Configuration.Abstractions;

public interface IAppDefinitionsBuilder
{
    IServiceCollection Services { get; }

    IAppDefinitionsBuilder RegisterDataProvider<TProvider, TProviderOptions>(string providerName)
        where TProvider : IDataProvider
        where TProviderOptions : class, new();
}
