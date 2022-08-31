using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.DataProviders;

public class FileSystemDataProvider : DataProviderBase<FileSystemDataProviderOptions>
{
    private PhysicalFileProvider? physicalFileProvider;

    public FileSystemDataProvider(IMemoryCache memoryCache)
        : base(memoryCache)
    {
    }

    protected override ContentAndChangeToken<string> GetContent(string fileName)
    {
        if (this.Options is null)
        {
            throw new InvalidOperationException("Missing options");
        }

        var provider = this.physicalFileProvider ??= new PhysicalFileProvider(this.Options.RootPath);

        return new ContentAndChangeToken<string>(
            File.ReadAllText(Path.Combine(this.Options.RootPath, fileName)),
            provider.Watch(fileName));
    }
}

public record FileSystemDataProviderOptions
{
    public string RootPath { get; init; } = null!;
}
