using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Riccardos77.AppConfig.Api.Configuration;
using Riccardos77.AppConfig.DataProviders;
using Riccardos77.AppConfig.DataProviders.Abstractions;

namespace Riccardos77.AppConfig.Api.Infrastructure
{
    public class PreloaderBackgroundService : BackgroundService
    {
        private readonly IEnumerable<IConfigureOptions<AppDefinitionOptions>> appDefinitions;
        private readonly IDataProviderFactory dataProviderFactory;
        private readonly HealthCheck healthCheck;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<PreloaderBackgroundService> logger;
        private Timer? timer;

        public PreloaderBackgroundService(
            IEnumerable<IConfigureOptions<AppDefinitionOptions>> appDefinitions,
            IDataProviderFactory dataProviderFactory,
            HealthCheck healthCheck,
            IMemoryCache memoryCache,
            ILogger<PreloaderBackgroundService> logger)
        {
            this.appDefinitions = appDefinitions;
            this.dataProviderFactory = dataProviderFactory;
            this.healthCheck = healthCheck;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.timer = new Timer(this.DoWork, null, TimeSpan.FromSeconds(1), Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            this.appDefinitions
                .OfType<ConfigureNamedOptions<AppDefinitionOptions>>()
                .Select(a => a.Name)
                .Distinct()
                .ToList()
                .ForEach(a =>
                {
                    try
                    {
                        var provider = this.dataProviderFactory.GetProvider(a);
                        provider.GetAppMetaschema(this.memoryCache);
                        provider.GetAppValues(this.memoryCache);
                        this.logger.LogDebug($"Preloading app {a} completed successfully");
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, $"Error preloading app {a}");
                    }
                });

            this.healthCheck.StartupCompleted = true;
        }
    }
}
