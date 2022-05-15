namespace Riccardos77.AppConfig.Abstractions;

public abstract class DataAccessProviderBase<TOptions> : IDataAccessProvider
    where TOptions : class, new()
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Allow derived classes to use typed Options")]
    protected readonly TOptions options = new();

    public string AppName { get; set; } = null!;

    public object Options => this.options;

    public virtual string GetAppMetaschemaContent()
    {
        return this.GetContent("Metaschema.json");
    }

    public virtual string GetAppValuesContent()
    {
        return this.GetContent("Values.json");
    }

    public virtual string GetFileContent(string fileName)
    {
        return this.GetContent(Path.Combine("Files", fileName));
    }

    protected abstract string GetContent(string fileName);
}
