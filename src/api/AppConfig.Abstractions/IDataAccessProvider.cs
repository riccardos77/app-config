namespace Riccardos77.AppConfig.Abstractions;

public interface IDataAccessProvider
{
    string AppName { get; set; }

    object Options { get; }

    string GetAppMetaschemaContent();

    string GetAppValuesContent();

    string GetFileContent(string fileName);
}
