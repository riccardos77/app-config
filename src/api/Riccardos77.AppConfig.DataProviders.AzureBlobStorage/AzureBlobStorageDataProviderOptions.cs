namespace Riccardos77.AppConfig.DataProviders.AzureBlobStorage;

public record AzureBlobStorageDataProviderOptions
{
    public string AccountName { get; set; } = null!;

    public string AccountKey { get; set; } = null!;

    public string? ContainerName { get; set; } = null!;
}
