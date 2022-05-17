using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.DataProviders;

public class FileSystemDataProvider : DataProviderBase<FileSystemDataProviderOptions>
{
    protected override string GetContent(string fileName)
    {
        return File.ReadAllText(Path.Combine(this.options.RootPath, fileName));
    }
}

public record FileSystemDataProviderOptions
{
    public string RootPath { get; init; } = null!;
}
