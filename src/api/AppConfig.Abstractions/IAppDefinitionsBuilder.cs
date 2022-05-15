using Microsoft.Extensions.DependencyInjection;

namespace Riccardos77.AppConfig.Abstractions;

public interface IAppDefinitionsBuilder
{
    IServiceCollection Services { get; }

    IAppDefinitionsBuilder RegisterDataAccessProvider<TProvider>(string providerName)
        where TProvider : IDataAccessProvider, new();
}
