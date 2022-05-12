namespace Riccardos77.AppConfig.Abstractions;

public abstract class DataAccessProviderBase<TOptions> : IDataAccessProvider
    where TOptions : class, new()
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Allow derived classes to use typed Options")]
    protected readonly TOptions options = new();

    public string AppName { get; set; } = null!;

    public object Options => this.options;

    public virtual string GetAppMetaschemaContent(string? appKey)
    {
        return this.GetContent("Metaschema.json", appKey, false);
    }

    public virtual string GetAppValuesContent(string? appKey)
    {
        return this.GetContent("Values.json", appKey, true);
    }

    public virtual string GetFileContent(string? appKey, string fileName)
    {
        return this.GetContent(Path.Combine("Files", fileName), appKey, true);
    }

    protected abstract string GetContent(string fileName, string? appKey, bool requireKey);
}
