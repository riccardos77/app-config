namespace Riccardos77.AppConfig.DataProviders.Abstractions;

public interface IDataProvider
{
    string AppName { get; set; }

    void SetOptions(object? options);

    ContentAndChangeToken<string> GetAppMetaschemaContent();

    ContentAndChangeToken<string> GetAppValuesContent();

    ContentAndChangeToken<byte[]> GetFileContent(string resourceFileName, string resourceId);
}
