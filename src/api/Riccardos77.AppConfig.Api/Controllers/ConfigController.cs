using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Riccardos77.AppConfig.DataProviders;
using Riccardos77.AppConfig.DataProviders.Abstractions;
using Riccardos77.AppConfig.Resources;
using Riccardos77.AppConfig.ValueManagers;
using System.Net.Mime;

namespace Riccardos77.AppConfig.Api.Controllers;

[Route("config")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly IDataProviderFactory dataProviderFactory;
    private readonly AppValuesSchemaGenerator appValuesSchemaGenerator;
    private readonly AppValuesInstanceSchemaGenerator appValuesInstanceSchemaGenerator;
    private readonly AppValuesInstanceParser appValuesInstanceParser;
    private readonly AppValueFileParser appValueFileParser;
    private readonly IMemoryCache memoryCache;

    public ConfigController(
        IDataProviderFactory dataProviderFactory,
        AppValuesSchemaGenerator appValuesSchemaGenerator,
        AppValuesInstanceSchemaGenerator appValuesInstanceSchemaGenerator,
        AppValuesInstanceParser appValuesInstanceParser,
        AppValueFileParser appValueFileParser,
        IMemoryCache memoryCache)
    {
        this.dataProviderFactory = dataProviderFactory;
        this.appValuesSchemaGenerator = appValuesSchemaGenerator;
        this.appValuesInstanceSchemaGenerator = appValuesInstanceSchemaGenerator;
        this.appValuesInstanceParser = appValuesInstanceParser;
        this.appValueFileParser = appValueFileParser;
        this.memoryCache = memoryCache;
    }

    [HttpGet("app-metaschema/schema")]
    [AllowAnonymous]
    public IActionResult GetAppMetaschemaSchema()
    {
        return this.JsonContent(SchemaResources.AppMetaschemaSchema.Replace("#schemaId#", Constants.JsonSchemaId.AppMetaschema.ToString()));
    }

    [HttpGet("cli/schema")]
    [AllowAnonymous]
    public IActionResult GetConfigCliSchema()
    {
        return this.JsonContent(SchemaResources.ConfigCliSchema.Replace("#schemaId#", Constants.JsonSchemaId.AppMetaschema.ToString()));
    }

    [HttpGet("apps/{appName}/metaschema")]
    [AllowAnonymous]
    public IActionResult GetAppMetaschema(string appName)
    {
        return this.JsonContent(this.dataProviderFactory.GetProvider(appName).GetAppMetaschemaContent().Content);
    }

    /*
    temporary removed until correct role will be defined

    [HttpGet("apps/{appName}/values")]
    [Authorize]
    public IActionResult GetAppValues(string appName)
    {
        return this.JsonContent(this.dataProviderFactory.GetProvider(appName).GetAppValuesContent(appKey));
    }
    */

    [HttpGet("apps/{appName}/values/schema")]
    public IActionResult GetAppValuesSchema(string appName)
    {
        var provider = this.dataProviderFactory.GetProvider(appName);
        var result = this.appValuesSchemaGenerator
            .Generate(provider.GetAppMetaschema(this.memoryCache))
            .ToString();

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}")]
    [Authorize]
    public IActionResult GetAppValuesInstance(string appName, string appIdentity, [FromQuery] Dictionary<string, string> queryParams)
    {
        var provider = this.dataProviderFactory.GetProvider(appName);
        var result = this.appValuesInstanceParser.Parse(
            provider.GetAppMetaschema(this.memoryCache),
            provider.GetAppValues(this.memoryCache),
            appIdentity,
            queryParams);

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/schema")]
    [AllowAnonymous]
    public IActionResult GetAppValuesInstanceSchema(string appName, string appIdentity)
    {
        var provider = this.dataProviderFactory.GetProvider(appName);
        var result = this.appValuesInstanceSchemaGenerator
            .Generate(provider.GetAppMetaschema(this.memoryCache), appIdentity)
            .ToString();

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/schema/tags")]
    [AllowAnonymous]
    public IActionResult GetAppValuesInstanceSchemaTags(string appName, string appIdentity)
    {
        var provider = this.dataProviderFactory.GetProvider(appName);
        var result = this.appValuesInstanceSchemaGenerator
            .GenerateSchemaTags(provider.GetAppMetaschema(this.memoryCache), appIdentity)
            .ToString();

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/files/{resourceFileName}/{resourceId}")]
    [Authorize]
    public IActionResult GetAppValuesFileInstance(
        string appName,
        string appIdentity,
        string resourceFileName,
        string resourceId,
        [FromQuery] Dictionary<string, string> queryParams,
        [FromHeader] string? appKey)
    {
        var fileContent = this.dataProviderFactory.GetProvider(appName).GetFileContent(resourceFileName);

        if (Path.GetExtension(resourceFileName) == ".html")
        {
            return this.Content(this.appValueFileParser.ParseHtml(fileContent.Content, resourceId, queryParams), MediaTypeNames.Text.Html);
        }
        else
        {
            return this.BadRequest();
        }
    }

    private IActionResult JsonContent(string json)
    {
        return this.Content(json, MediaTypeNames.Application.Json);
    }
}
