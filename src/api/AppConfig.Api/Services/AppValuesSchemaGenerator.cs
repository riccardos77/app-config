using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Riccardos77.AppConfig.Api.Models;

namespace Riccardos77.AppConfig.Api.Services;

internal class AppValuesSchemaGenerator
{
    public static JSchema Generate(AppMetaschema conf)
    {
        var fullSchema = new JSchema
        {
            AllowAdditionalProperties = false,
        };

        foreach (var item in conf.Schemas)
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

        var schemaTags = InsertSchemaTags(conf);
        schemaDefs["sys:tags"] = schemaTags;

        return schemaTags;

        static JSchema InsertSchemaTags(AppMetaschema conf)
        {
            var schemaTags = new JSchema
            {
                Type = JSchemaType.Object,
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
}
