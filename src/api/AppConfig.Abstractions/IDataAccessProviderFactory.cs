namespace Riccardos77.AppConfig.Abstractions;

public interface IDataAccessProviderFactory
{
    IDataAccessProvider GetProvider(string appName);
}
