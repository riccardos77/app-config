using Microsoft.Extensions.Azure;
using Riccardos77.AppConfig.Abstractions;

namespace Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage;

public static class DataAccessProviderFactoryBuilderExtensions
{
    public static IDataAccessProviderFactoryBuilder RegisterAzureBlobStorage(this IDataAccessProviderFactoryBuilder builder, string configSection = "DataAccess:AzureBlobStorage")
    {
        builder.RegisterProvider<AzureBlobStorageDataAccessProvider>(configSection);

        builder.Services.AddAzureClientsCore();

        return builder;
    }
}
