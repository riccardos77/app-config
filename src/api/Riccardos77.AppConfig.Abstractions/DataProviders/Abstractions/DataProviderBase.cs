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
        return this.memoryCache.GetOrCreate(new FileContentCacheKey("metaschema.json"), this.GetContentCacheFactory);
    }

    public virtual ContentAndChangeToken<string> GetAppValuesContent()
    {
        return this.memoryCache.GetOrCreate(new FileContentCacheKey("values.json"), this.GetContentCacheFactory);
    }

    public virtual ContentAndChangeToken<string> GetFileContent(string fileName)
    {
        return this.memoryCache.GetOrCreate(new FileContentCacheKey(fileName), this.GetContentCacheFactory);
    }

    protected abstract ContentAndChangeToken<string> GetContent(string fileName);

    private ContentAndChangeToken<string> GetContentCacheFactory(ICacheEntry cacheEntry)
    {
        if (cacheEntry.Key is FileContentCacheKey key)
        {
            var contentAndToken = this.GetContent(key.FileName);

            cacheEntry.AddExpirationToken(contentAndToken.ChangeToken);
            return contentAndToken;
        }
        else
        {
            throw new InvalidOperationException("Invalid cache key");
        }
    }

    private record FileContentCacheKey(string FileName);
}
