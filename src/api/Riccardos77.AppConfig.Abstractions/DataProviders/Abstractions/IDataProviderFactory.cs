namespace Riccardos77.AppConfig.DataProviders.Abstractions;

public interface IDataProviderFactory
{
    IDataProvider GetProvider(string appName);
}
