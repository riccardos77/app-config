using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig;

public static class HelperExtensions
{
    public static Dictionary<string, ValueSchema> ForIdentity(this Dictionary<string, ValueSchema> schemas, string appIdentity)
    {
        return schemas.Where(s => s.Value.EnabledIdentities.Contains(appIdentity)).ToDictionary(s => s.Key, s => s.Value);
    }

    public static string GetCacheKey(this Dictionary<string, string> tags)
    {
        return tags.Aggregate(string.Empty, (acc, t) => acc + t.Key + "=" + t.Value + "|");
    }
}
