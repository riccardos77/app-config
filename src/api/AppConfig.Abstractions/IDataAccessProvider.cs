namespace Riccardos77.AppConfig.Abstractions;

public interface IDataAccessProvider
{
    string AppName { get; set; }

    object Options { get; }

    string GetAppMetaschemaContent(string? appKey);

    string GetAppValuesContent(string? appKey);

    string GetFileContent(string? appKey, string fileName);
}
