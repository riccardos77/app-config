namespace Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage;

public record AzureBlobStorageDataAccessProviderOptions
{
    public string AccountName { get; set; } = null!;

    public string AccountKey { get; set; } = null!;

    public string? ContainerName { get; set; } = null!;
}
