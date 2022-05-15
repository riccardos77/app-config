using Microsoft.Extensions.Azure;
using Riccardos77.AppConfig.Abstractions;

namespace Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage;

public static class AppDefinitionsBuilderExtensions
{
    public static IAppDefinitionsBuilder RegisterAzureBlobStorage(this IAppDefinitionsBuilder builder, string providerName = "AzureBlobStorage")
    {
        builder.RegisterDataAccessProvider<AzureBlobStorageDataAccessProvider>(providerName);

        builder.Services.AddAzureClientsCore();

        return builder;
    }
}
