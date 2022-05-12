namespace Riccardos77.AppConfig.Api;

public static class Constants
{
    public static class JsonSchemaId
    {
        public static readonly string ConfigRoot = "http://localhost:81/config";

        public static readonly Uri JsonDraft04 = new("http://json-schema.org/draft-04/schema");
        public static readonly Uri AppMetaschema = new($"{ConfigRoot}/app-metaschema/schema");
        public static readonly Uri AppMetaschema2 = new($"http://localhost:8080/config/app-metaschema/schema");
        public static readonly Uri ConfigCli = new($"{ConfigRoot}/cli/schema");

        public static Uri AppValuesInstance(string appName, string appIdentity)
        {
            return new($"{ConfigRoot}/apps/{appName}/values/instances/{appIdentity}/schema");
        }
    }
}
