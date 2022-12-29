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

    protected override ContentAndChangeToken<string> GetTextContent(string fileName, string? resourceId)
    {
        if (this.Options is null)
        {
            throw new InvalidOperationException("Missing options");
        }

        var provider = this.physicalFileProvider ??= new PhysicalFileProvider(this.Options.RootPath);
        var foundFileName = this.GetFileName(fileName, resourceId);

        return new ContentAndChangeToken<string>(
            File.ReadAllText(Path.Combine(this.Options.RootPath, foundFileName)),
            provider.Watch(foundFileName));
    }

    protected override ContentAndChangeToken<byte[]> GetBinaryContent(string fileName, string? resourceId)
    {
        if (this.Options is null)
        {
            throw new InvalidOperationException("Missing options");
        }

        var provider = this.physicalFileProvider ??= new PhysicalFileProvider(this.Options.RootPath);
        var foundFileName = this.GetFileName(fileName, resourceId);

        return new ContentAndChangeToken<byte[]>(
            File.ReadAllBytes(Path.Combine(this.Options.RootPath, foundFileName)),
            provider.Watch(foundFileName));
    }

    private string GetFileName(string fileName, string? resourceId)
    {
        if (this.Options is null)
        {
            throw new InvalidOperationException("Missing options");
        }

        var fileNames = new[]
        {
            $"{fileName}--{resourceId}",
            Path.Combine(fileName, resourceId ?? string.Empty),
            fileName,
        };

        var foundFileName = fileNames.FirstOrDefault(f => File.Exists(Path.Combine(this.Options.RootPath, f)));
        if (!string.IsNullOrEmpty(foundFileName))
        {
            return foundFileName;
        }
        else
        {
            throw new FileNotFoundException($"No valid file found for {fileName} and {resourceId}");
        }
    }
}

public record FileSystemDataProviderOptions
{
    public string RootPath { get; init; } = null!;
}
