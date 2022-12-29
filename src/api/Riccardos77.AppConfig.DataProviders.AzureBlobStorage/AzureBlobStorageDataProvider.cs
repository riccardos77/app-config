// using Azure.Storage;
// using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Memory;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.DataProviders.AzureBlobStorage;

public class AzureBlobStorageDataProvider : DataProviderBase<AzureBlobStorageDataProviderOptions>
{
    public AzureBlobStorageDataProvider(IMemoryCache memoryCache)
        : base(memoryCache)
    {
    }

    protected override ContentAndChangeToken<string> GetTextContent(string fileName, string? resourceId)
    {
        throw new NotImplementedException();

        // var containerName = this.options.ContainerName ?? this.AppName;
        // var accountName = this.options.AccountName;
        // var accountKey = this.options.AccountKey;

        // var client = new BlobClient(
        //    new Uri($"https://{accountName}.blob.core.windows.net/{containerName}/{fileName}"),
        //    new StorageSharedKeyCredential(accountName, accountKey));

        // return client.DownloadContent().Value.Content.ToString();
    }

    protected override ContentAndChangeToken<byte[]> GetBinaryContent(string fileName, string? resourceId)
    {
        throw new NotImplementedException();
    }
}
