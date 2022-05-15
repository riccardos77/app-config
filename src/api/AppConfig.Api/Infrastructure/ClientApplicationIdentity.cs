using System.Security.Claims;

namespace Riccardos77.AppConfig.Api.Infrastructure
{
    public class ClientApplicationIdentity : ClaimsIdentity
    {
        public ClientApplicationIdentity(string appName, string appIdentity)
           : base(ApiKeyAuthenticationHandler.SchemeName)
        {
            this.AppName = appName;
            this.AppIdentity = appIdentity;
        }

        public string AppName { get; }

        public string AppIdentity { get; }

        public override string? Name => $"{this.AppName}:{this.AppIdentity}";
    }
}
