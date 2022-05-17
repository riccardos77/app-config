using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riccardos77.AppConfig.DataProviders.Abstractions;
using Riccardos77.AppConfig.Resources;
using Riccardos77.AppConfig.ValueManagers;
using Riccardos77.AppConfig.ValueManagers.Models;
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

    public ConfigController(
        IDataProviderFactory dataProviderFactory,
        AppValuesSchemaGenerator appValuesSchemaGenerator,
        AppValuesInstanceSchemaGenerator appValuesInstanceSchemaGenerator,
        AppValuesInstanceParser appValuesInstanceParser,
        AppValueFileParser appValueFileParser)
    {
        this.dataProviderFactory = dataProviderFactory;
        this.appValuesSchemaGenerator = appValuesSchemaGenerator;
        this.appValuesInstanceSchemaGenerator = appValuesInstanceSchemaGenerator;
        this.appValuesInstanceParser = appValuesInstanceParser;
        this.appValueFileParser = appValueFileParser;
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
        return this.JsonContent(this.dataProviderFactory.GetProvider(appName).GetAppMetaschemaContent());
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
        return this.JsonContent(this.appValuesSchemaGenerator.Generate(this.LoadAppMetaschema(appName)).ToString());
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}")]
    [Authorize]
    public IActionResult GetAppValuesInstance(string appName, string appIdentity, [FromQuery] Dictionary<string, string> queryParams)
    {
        var result = this.appValuesInstanceParser.Parse(
            this.LoadAppMetaschema(appName),
            this.dataProviderFactory.GetProvider(appName).GetAppValuesContent(),
            appIdentity,
            queryParams);

        return this.JsonContent(result);
    }

    [HttpGet("apps/{appName}/values/instances/{appIdentity}/schema")]
    [AllowAnonymous]
    public IActionResult GetAppValuesInstanceSchema(string appName, string appIdentity)
    {
        return this.JsonContent(this.appValuesInstanceSchemaGenerator.Generate(this.LoadAppMetaschema(appName), appIdentity).ToString());
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
            return this.Content(this.appValueFileParser.ParseHtml(fileContent, resourceId, queryParams), MediaTypeNames.Text.Html);
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

    private AppMetaschema LoadAppMetaschema(string appName)
    {
        return AppMetaschema.Load(
            appName,
            this.dataProviderFactory.GetProvider(appName).GetAppMetaschemaContent());
    }
}
