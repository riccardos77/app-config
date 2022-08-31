namespace Riccardos77.AppConfig.Api.Configuration;

public class AppDefinitionOptions
{
    public Dictionary<string, string> IdentityKeys { get; set; } = new();

    public Type? ProviderType { get; set; }

    public object? ProviderOptions { get; set; }
}
