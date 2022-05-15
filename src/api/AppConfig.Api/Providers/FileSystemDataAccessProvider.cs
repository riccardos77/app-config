using Riccardos77.AppConfig.Abstractions;

namespace Riccardos77.AppConfig.Api.Providers;

public class FileSystemDataAccessProvider : DataAccessProviderBase<FileSystemDataAccessProviderOptions>
{
    protected override string GetContent(string fileName)
    {
        return File.ReadAllText(Path.Combine(this.options.RootPath, fileName));
    }
}

public record FileSystemDataAccessProviderOptions
{
    public string RootPath { get; init; } = null!;
}
