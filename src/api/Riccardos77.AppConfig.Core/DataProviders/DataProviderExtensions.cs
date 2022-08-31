using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Riccardos77.AppConfig.DataProviders.Abstractions;
using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig.DataProviders
{
    public static class DataProviderExtensions
    {
        public static AppMetaschema GetAppMetaschema(this IDataProvider dataProvider, IMemoryCache memoryCache)
        {
            return memoryCache.GetOrCreate(
                $"AppMetaschema_{dataProvider.AppName}",
                entry =>
                {
                    var contentAndToken = dataProvider.GetAppMetaschemaContent();
                    entry.AddExpirationToken(contentAndToken.ChangeToken);
                    entry.RegisterPostEvictionCallback((key, value, reason, state) => GetAppMetaschema(dataProvider, memoryCache));
                    return AppMetaschema.Load(dataProvider.AppName, contentAndToken.Content);
                });
        }

        public static JObject GetAppValues(this IDataProvider dataProvider, IMemoryCache memoryCache)
        {
            return memoryCache.GetOrCreate(
                $"AppValues_{dataProvider.AppName}",
                entry =>
                {
                    var contentAndToken = dataProvider.GetAppValuesContent();
                    entry.AddExpirationToken(contentAndToken.ChangeToken);
                    entry.RegisterPostEvictionCallback((key, value, reason, state) => GetAppValues(dataProvider, memoryCache));
                    return JsonConvert.DeserializeObject<JObject>(contentAndToken.Content) ?? throw new InvalidOperationException("Deserialization error");
                });
        }
    }
}
