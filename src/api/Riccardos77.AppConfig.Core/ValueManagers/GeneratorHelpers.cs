using Newtonsoft.Json.Schema;
using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig.ValueManagers;

internal class GeneratorHelpers
{
    internal static JSchema InsertSchemaTags(AppMetaschema conf, JSchemaType? type)
    {
        var schemaTags = new JSchema
        {
            Type = type,
            AllowAdditionalProperties = false
        };

        if (conf.Tags is not null)
        {
            foreach (var confTag in conf.Tags)
            {
                var schemaTag = schemaTags.Properties[confTag.Name] = new JSchema()
                {
                    Type = JSchemaType.String,
                    AllowAdditionalProperties = false,
                };

                foreach (var confTagValue in confTag.Values)
                {
                    schemaTag.Enum.Add(confTagValue);
                }
            }
        }

        return schemaTags;
    }
}
