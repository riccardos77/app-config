using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Riccardos77.AppConfig.Api.Configuration;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Riccardos77.AppConfig.Api.Infrastructure;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly IOptionsSnapshot<AppDefinitionOptions> appDefinitionsOptions;

    public ApiKeyAuthenticationHandler(
        IOptionsSnapshot<AppDefinitionOptions> appDefinitionsOptions,
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        this.appDefinitionsOptions = appDefinitionsOptions;
    }

    public static string SchemeName => "ApiKey";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authKey = this.Context.Request.Headers.Authorization.ToString().Split(" ").Last();
        var appName = this.Context.GetRouteValue("AppName") as string;
        var appIdentity = this.Context.GetRouteValue("AppIdentity") as string;

        if (!string.IsNullOrEmpty(authKey) && !string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(appIdentity))
        {
            if (this.appDefinitionsOptions.Get(appName).IdentityKeys.TryGetValue(appIdentity, out var identityKey)
                && authKey == identityKey)
            {
                var identity = new ClientApplicationIdentity(appName, appIdentity);
                identity.AddClaim(new Claim(ClaimTypes.Role, "ClientApp"));

                var principal = new ClaimsPrincipal(identity);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
            }
        }

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
}
