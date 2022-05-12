using Riccardos77.AppConfig.Api.Models;

namespace Riccardos77.AppConfig.Api;

internal static class ExtensionMethods
{
    internal static Dictionary<string, ValueSchema> ForIdentity(this Dictionary<string, ValueSchema> schemas, string appIdentity)
    {
        return schemas.Where(s => s.Value.EnabledIdentities.Contains(appIdentity)).ToDictionary(s => s.Key, s => s.Value);
    }
}
