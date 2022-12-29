using Microsoft.Extensions.Caching.Memory;

namespace Riccardos77.AppConfig.DataProviders.Abstractions;

public abstract class DataProviderBase<TOptions> : IDataProvider
    where TOptions : class, new()
{
    private readonly IMemoryCache memoryCache;

    public DataProviderBase(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public string AppName { get; set; } = null!;

    protected TOptions? Options { get; set; }

    public virtual void SetOptions(object? options)
    {
        if (options is TOptions typedOptions)
        {
            this.Options = typedOptions;
        }
        else
        {
            throw new InvalidOperationException("Options instance type unexpected");
        }
    }

    public virtual ContentAndChangeToken<string> GetAppMetaschemaContent()
    {
        return this.memoryCache.GetOrCreate(new FileContentCacheKey("metaschema.json", null), this.GetTextContentCacheFactory);
    }

    public virtual ContentAndChangeToken<string> GetAppValuesContent()
    {
        return this.memoryCache.GetOrCreate(new FileContentCacheKey("values.json", null), this.GetTextContentCacheFactory);
    }

    public virtual ContentAndChangeToken<byte[]> GetFileContent(string resourceFileName, string resourceId)
    {
        return this.memoryCache.GetOrCreate(new FileContentCacheKey(resourceFileName, resourceId), this.GetBinaryContentCacheFactory);
    }

    protected abstract ContentAndChangeToken<string> GetTextContent(string fileName, string? resourceId);

    protected abstract ContentAndChangeToken<byte[]> GetBinaryContent(string fileName, string? resourceId);

    private ContentAndChangeToken<string> GetTextContentCacheFactory(ICacheEntry cacheEntry)
    {
        if (cacheEntry.Key is FileContentCacheKey key)
        {
            var contentAndToken = this.GetTextContent(key.FileName, key.ResourceId);

            cacheEntry.AddExpirationToken(contentAndToken.ChangeToken);
            return contentAndToken;
        }
        else
        {
            throw new InvalidOperationException("Invalid cache key");
        }
    }

    private ContentAndChangeToken<byte[]> GetBinaryContentCacheFactory(ICacheEntry cacheEntry)
    {
        if (cacheEntry.Key is FileContentCacheKey key)
        {
            var contentAndToken = this.GetBinaryContent(key.FileName, key.ResourceId);

            cacheEntry.AddExpirationToken(contentAndToken.ChangeToken);
            return contentAndToken;
        }
        else
        {
            throw new InvalidOperationException("Invalid cache key");
        }
    }

    private record FileContentCacheKey(string FileName, string? ResourceId);
}
