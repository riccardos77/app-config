using Microsoft.Extensions.Azure;
using Riccardos77.AppConfig.Configuration.Abstractions;

namespace Riccardos77.AppConfig.DataProviders.AzureBlobStorage;

public static class AppDefinitionsBuilderExtensions
{
    public static IAppDefinitionsBuilder RegisterAzureBlobStorageDataProvider(this IAppDefinitionsBuilder builder, string providerName = "AzureBlobStorage")
    {
        builder.RegisterDataProvider<AzureBlobStorageDataProvider>(providerName);

        builder.Services.AddAzureClientsCore();

        return builder;
    }
}
