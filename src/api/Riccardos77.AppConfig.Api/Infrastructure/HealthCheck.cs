using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Riccardos77.AppConfig.Api.Infrastructure
{
    public class HealthCheck : IHealthCheck
    {
        private readonly ILogger<HealthCheck> logger;

        public HealthCheck(ILogger<HealthCheck> logger)
        {
            this.logger = logger;
        }

        public bool StartupCompleted { get; set; }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (this.StartupCompleted)
            {
                this.logger.LogDebug("Startup completed. HealthCheck returns Healthy");
                return Task.FromResult(HealthCheckResult.Healthy("The startup task has completed."));
            }

            this.logger.LogWarning("Startup pending. HealthCheck returns Unhealthy");
            return Task.FromResult(HealthCheckResult.Unhealthy("That startup task is still running."));
        }
    }
}
