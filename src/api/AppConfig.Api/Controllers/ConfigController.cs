using Microsoft.AspNetCore.Mvc;
using Riccardos77.AppConfig.Abstractions;
using Riccardos77.AppConfig.Api.Models;
using Riccardos77.AppConfig.Api.Resources;
using Riccardos77.AppConfig.Api.Services;
using System.Net.Mime;

namespace Riccardos77.AppConfig.Api.Controllers;

[Route("config")]
[ApiController]
public class ConfigController : ControllerBase
{
    private readonly IDataAccessProviderFactory dataAccessProviderFactory;

    public ConfigController(IDataAccessProviderFactory dataAccessProviderFactory)
    {
        this.dataAccessProviderFactory = dataAccessProviderFactory;
    }

    [HttpGet("app-metaschema/schema")]
    public IActionResult GetAppMetaschemaSchema()
    {
        return this.JsonContent(CoreResources.AppMetaschemaSchema.Replace("#schemaId#", Constants.JsonSchemaId.AppMetaschema.ToString()));
    }

    [HttpGet("cli/schema")]
    public IActionResult GetConfigCliSchema()
    {
        return this.JsonContent(CoreResources.ConfigCliSchema.Replace("#schemaId#", Constants.JsonSchemaId.AppMetaschema.ToString()));
    }

    [HttpGet("apps/{appName}/metaschema")]
    public IActionResult GetAppMetaschema(string appName, [FromHeader] string? appKey)
    {
        return this.JsonContent(this.dataAccessProviderFactory.GetProvider(appName).GetAppMetaschemaContent(appKey));
    }

    [HttpGet("apps/{appName}/values")]
    public IActionResult GetAppValues(string appName, [FromHeader] string? appKey)
    {
        return this.JsonContent(this.dataAccessProviderFactory.GetProvider(appName).GetAppValuesContent(appKey));
    }

    [HttpGet("apps/{appName}/values/schema")]
    public IActionResult GetAppValuesSchema(string appName, [FromHeader] string? appKey)
    {
        return this.JsonContent(AppValuesSchemaGenerator.Generate(this.LoadAppMetaschema(appName, appKey)).ToString());
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}")]
    public IActionResult GetAppValuesInstance(string appName, string appIdentity, [FromQuery] Dictionary<string, string> queryParams, [FromHeader] string? appKey)
    {
        var result = AppValuesInstanceParser.Parse(
            this.LoadAppMetaschema(appName, appKey),
            this.dataAccessProviderFactory.GetProvider(appName).GetAppValuesContent(appKey),
            appIdentity,
            queryParams);

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/schema")]
    public IActionResult GetAppValuesInstanceSchema(string appName, string appIdentity, [FromHeader] string? appKey)
    {
        return this.JsonContent(AppValuesInstanceSchemaGenerator.Generate(this.LoadAppMetaschema(appName, appKey), appIdentity).ToString());
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/files/{resourceFileName}/{resourceId}")]
    public IActionResult GetAppValuesFileInstance(
        string appName,
        string appIdentity,
        string resourceFileName,
        string resourceId,
        [FromQuery] Dictionary<string, string> queryParams,
        [FromHeader] string? appKey)
    {
        var fileContent = this.dataAccessProviderFactory.GetProvider(appName).GetFileContent(appKey, resourceFileName);

        if (Path.GetExtension(resourceFileName) == ".html")
        {
            return this.Content(AppValueFileParser.ParseHtml(fileContent, resourceId, queryParams), MediaTypeNames.Text.Html);
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

    private AppMetaschema LoadAppMetaschema(string appName, string? appKey)
    {
        return AppMetaschema.Load(
            appName,
            this.dataAccessProviderFactory.GetProvider(appName).GetAppMetaschemaContent(appKey));
    }
}
