using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Riccardos77.AppConfig.Api.Infrastructure
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddAppConfigHealthChecks(this IServiceCollection services)
        {
            services.AddSingleton<HealthCheck>();
            services.AddHealthChecks().AddCheck<HealthCheck>("StartupCheck");

            return services;
        }

        public static IEndpointRouteBuilder MapAppConfigHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints
                .MapHealthChecks(
                    "/readiness",
                    new HealthCheckOptions()
                    {
                        ResultStatusCodes =
                        {
                            [HealthStatus.Healthy] = StatusCodes.Status200OK,
                            [HealthStatus.Degraded] = StatusCodes.Status200OK,
                            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                        },
                    })
                .RequireHost("*:2113");

            return endpoints;
        }
    }
}
