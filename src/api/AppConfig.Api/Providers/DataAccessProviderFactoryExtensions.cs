using Riccardos77.AppConfig.Abstractions;

namespace Riccardos77.AppConfig.Api.Providers;

public static class DataAccessProviderFactoryExtensions
{
    public static IServiceCollection AddDataAccessProviderFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IDataAccessProviderFactoryBuilder> builder)
    {
        var factory = new DataAccessProviderFactory();

        var builderInstance = new Builder(services);
        builder(builderInstance);

        foreach (var provider in builderInstance.Providers)
        {
            configuration.GetSection(provider.Key)
                .GetChildren()
                .ToList()
                .ForEach(c =>
                {
                    if (Activator.CreateInstance(provider.Value) is IDataAccessProvider providerInstance)
                    {
                        providerInstance.AppName = provider.Key;
                        c.Bind(providerInstance.Options);

                        factory.RegisterProvider(c.Key, providerInstance);
                    }
                });
        }

        services.AddSingleton<IDataAccessProviderFactory>(factory);

        return services;
    }

    private class Builder : IDataAccessProviderFactoryBuilder
    {
        public Builder(IServiceCollection services)
        {
            this.Services = services;
        }

        public Dictionary<string, Type> Providers { get; } = new();

        public IServiceCollection Services { get; }

        public IDataAccessProviderFactoryBuilder RegisterProvider<TProvider>(string configSection)
            where TProvider : IDataAccessProvider, new()
        {
            this.Providers.Add(configSection, typeof(TProvider));

            return this;
        }
    }
}
