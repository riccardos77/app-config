using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Riccardos77.AppConfig.ValueManagers.Models;

namespace Riccardos77.AppConfig.ValueManagers;

public class AppValuesSchemaGenerator
{
    public JSchema Generate(AppMetaschema conf)
    {
        var fullSchema = new JSchema
        {
            AllowAdditionalProperties = false,
        };

        foreach (var item in conf.Schemas)
        {
            if (item.Value.Nullable && item.Value.Schema.OneOf.Count < 2)
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

        var schemaTags = AddDefinitions(conf, fullSchema);
        AddProperties(conf, fullSchema, schemaTags);

        return fullSchema;
    }

    private static void AddProperties(AppMetaschema conf, JSchema fullSchema, JSchema schemaTags)
    {
        fullSchema.Properties.Add("$schema", new JSchema() { Type = JSchemaType.String });

        foreach (var confSchema in conf.Schemas)
        {
            var jSchema = GetOrCreateJSchemaForConf(confSchema.Key, fullSchema);
            jSchema.AllowAdditionalProperties = true;
            jSchema.OneOf.Add(new JSchema() { Ref = confSchema.Value.Schema });
            jSchema.OneOf.Add(new JSchema()
            {
                Type = JSchemaType.Array,
                Items =
                {
                    new JSchema()
                    {
                        Type = JSchemaType.Object,
                        Properties =
                        {
                            ["tags"] = new JSchema() { Ref = schemaTags },
                            ["value"] = new JSchema() { Ref = confSchema.Value.Schema },
                        },
                        AllowAdditionalProperties = false,
                    },
                },
            });
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

    private static JSchema AddDefinitions(AppMetaschema conf, JSchema valueSchema)
    {
        var schemaDefs = new JObject();
        valueSchema.ExtensionData["definitions"] = schemaDefs;

        foreach (var confSchema in conf.Schemas)
        {
            schemaDefs[confSchema.Key] = confSchema.Value.Schema;
        }

        var schemaTags = GeneratorHelpers.InsertSchemaTags(conf, JSchemaType.Object);
        schemaDefs["sys:tags"] = schemaTags;

        return schemaTags;
    }
}
