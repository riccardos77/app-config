using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig;

internal static class HelperExtensions
{
    internal static Dictionary<string, ValueSchema> ForIdentity(this Dictionary<string, ValueSchema> schemas, string appIdentity)
    {
        return schemas.Where(s => s.Value.EnabledIdentities.Contains(appIdentity)).ToDictionary(s => s.Key, s => s.Value);
    }
}
