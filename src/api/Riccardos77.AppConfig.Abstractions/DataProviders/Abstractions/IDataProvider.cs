namespace Riccardos77.AppConfig.DataProviders.Abstractions;

public interface IDataProvider
{
    string AppName { get; set; }

    object Options { get; }

    string GetAppMetaschemaContent();

    string GetAppValuesContent();

    string GetFileContent(string fileName);
}
