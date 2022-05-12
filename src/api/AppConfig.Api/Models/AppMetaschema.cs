using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Riccardos77.AppConfig.Api.Resources;

namespace Riccardos77.AppConfig.Api.Models;

public record AppMetaschema
{
    static AppMetaschema()
    {
        MetaschemaSchemaResolver = new JSchemaPreloadedResolver();
        MetaschemaSchemaResolver.Add(Constants.JsonSchemaId.AppMetaschema, CoreResources.AppMetaschemaSchema);
        MetaschemaSchemaResolver.Add(Constants.JsonSchemaId.AppMetaschema2, CoreResources.AppMetaschemaSchema);
        MetaschemaSchemaResolver.Add(Constants.JsonSchemaId.JsonDraft04, CoreResources.JsonDraft04Schema);
    }

    public static JSchema MetaschemaSchema { get; private set; } = null!;

    public static JSchemaPreloadedResolver MetaschemaSchemaResolver { get; private set; } = null!;

    public string AppName { get; set; } = null!;

    public Dictionary<string, AppIdentity>? AppIdentities { get; set; } = null!;

    public Tag[]? Tags { get; set; } = null!;

    public Dictionary<string, ValueSchema> Schemas { get; set; } = null!;

    public static AppMetaschema Load(string appName, string appMetaschemaJson)
    {
        MetaschemaSchema = JSchema.Parse(CoreResources.AppMetaschemaSchema, MetaschemaSchemaResolver);

        var schema = JsonConvert.DeserializeObject<AppMetaschema>(appMetaschemaJson) ?? throw new InvalidOperationException("Error reading conf");
        schema.AppName = appName;

        return schema;
    }
}

public record AppIdentity(string AadApplicationId, string DisplayName);

public record Tag(string Name, string[] Values);

public record ValueSchema
{
    [JsonConstructor]
    public ValueSchema(string[] enabledIdentities, bool nullable, JObject schema)
    {
        this.EnabledIdentities = enabledIdentities;
        this.Nullable = nullable;
        this.Schema = JSchema.Parse(schema.ToString(), AppMetaschema.MetaschemaSchemaResolver);

        // this.Schema = JSchema.Parse(
        //    schema.ToString(),
        //    new JSchemaReaderSettings { ResolveSchemaReferences = false });

        // var settings = new JSchemaWriterSettings
        // {
        //    ReferenceHandling = JSchemaWriterReferenceHandling.Always,
        //    ExternalSchemas = new List<ExternalSchema> {
        //        new(Constants.JsonSchemaId.AppMetaschema2, JSchema.Parse(CoreResources.AppMetaschemaSchema, AppMetaschema.MetaschemaSchemaResolver))
        //    }
        // };
        // var a = this.Schema.ToString(settings);
    }

    public string[] EnabledIdentities { get; init; }

    public bool Nullable { get; init; }

    public JSchema Schema { get; set; } = null!;
}
