using Microsoft.Extensions.DependencyInjection;

namespace Riccardos77.AppConfig.Abstractions;

public interface IDataAccessProviderFactoryBuilder
{
    IServiceCollection Services { get; }

    IDataAccessProviderFactoryBuilder RegisterProvider<TProvider>(string configSection)
        where TProvider : IDataAccessProvider, new();
}
