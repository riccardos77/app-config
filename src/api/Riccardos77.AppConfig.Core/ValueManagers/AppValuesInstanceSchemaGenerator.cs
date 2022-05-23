using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig.ValueManagers;

public class AppValuesInstanceSchemaGenerator
{
    public JSchema Generate(AppMetaschema metaschema, string appIdentity)
    {
        var fullSchema = new JSchema
        {
            AllowAdditionalProperties = false,
            Id = Constants.JsonSchemaId.AppValuesInstance(metaschema.AppName, appIdentity),
            SchemaVersion = Constants.JsonSchemaId.JsonDraft04,
            Title = $"{appIdentity} config",
        };

        var filteredConfSchemas = metaschema.Schemas.ForIdentity(appIdentity);

        foreach (var item in filteredConfSchemas)
        {
            if (item.Value.Nullable)
            {
                item.Value.Schema = new JSchema()
                {
                    OneOf =
                    {
                        item.Value.Schema,
                        new JSchema() { Type = JSchemaType.Null },
                    },
                };
            }
        }

        AddDefinitions(filteredConfSchemas, fullSchema);
        AddProperties(filteredConfSchemas, fullSchema);

        return fullSchema;
    }

    public JSchema GenerateSchemaTags(AppMetaschema metaschema, string appIdentity)
    {
        var fullSchema = GeneratorHelpers.InsertSchemaTags(metaschema, null);
        fullSchema.Title = $"{appIdentity} config tags";
        fullSchema.SchemaVersion = Constants.JsonSchemaId.JsonDraft04;

        return fullSchema;
    }

    private static void AddProperties(Dictionary<string, ValueSchema> confSchemas, JSchema fullSchema)
    {
        fullSchema.Properties.Add("$schema", new JSchema() { Type = JSchemaType.String });

        foreach (var confSchema in confSchemas)
        {
            var jSchema = GetOrCreateJSchemaForConf(confSchema.Key, fullSchema);
            jSchema.Ref = confSchema.Value.Schema;
        }
    }

    private static JSchema GetOrCreateJSchemaForConf(string key, JSchema schema)
    {
        foreach (var keyPart in key.Split("."))
        {
            if (schema.Properties.ContainsKey(keyPart))
            {
                schema = schema.Properties[keyPart];
            }
            else
            {
                schema.Properties[keyPart] = new JSchema() { AllowAdditionalProperties = false };
                schema.Required.Add(keyPart);
                schema = schema.Properties[keyPart];
            }
        }

        return schema;
    }

    private static void AddDefinitions(Dictionary<string, ValueSchema> confSchemas, JSchema valueSchema)
    {
        var schemaDefs = new JObject();
        valueSchema.ExtensionData["definitions"] = schemaDefs;

        foreach (var confSchema in confSchemas)
        {
            schemaDefs[confSchema.Key] = confSchema.Value.Schema;
        }
    }
}
