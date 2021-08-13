namespace MASA.Contrib.ReadWriteSpliting.CQRS.Dapr
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddDaprEventBus<TIntegrationEventLogService>(this ServiceCollection services)
            where TIntegrationEventLogService : class, IIntegrationEventLogService
        {
            services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
            services.AddScoped<IIntegrationEventLogService, TIntegrationEventLogService>();

            // check AppConfig is configured
            if (services.BuildServiceProvider().GetService<IOptionsMonitor<AppConfig>>() is null)
                throw new Exception("Please configure the AppConfig options first.");

            return services;
        }
    }
}
