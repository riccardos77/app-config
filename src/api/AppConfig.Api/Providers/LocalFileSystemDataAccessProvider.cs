using Riccardos77.AppConfig.Abstractions;

namespace Riccardos77.AppConfig.Api.Providers;

public class LocalFileSystemDataAccessProvider : DataAccessProviderBase<LocalFileSystemDataAccessProviderOptions>
{
    protected override string GetContent(string fileName, string? appKey, bool requireKey)
    {
        if (requireKey && appKey != this.options.AppKey)
        {
            throw new InvalidOperationException("Invalid AppKey");
        }

        return File.ReadAllText(Path.Combine(this.options.RootPath, fileName));
    }
}

public record LocalFileSystemDataAccessProviderOptions
{
    public string RootPath { get; init; } = null!;

    public string AppKey { get; init; } = null!;
}
