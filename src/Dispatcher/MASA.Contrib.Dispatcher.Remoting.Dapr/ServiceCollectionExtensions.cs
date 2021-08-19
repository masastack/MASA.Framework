namespace MASA.Contrib.Dispatcher.Remoting.Dapr
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddDaprEventBus<TIntegrationEventLogService>(this ServiceCollection services)
            where TIntegrationEventLogService : class, IIntegrationEventLogService
        {
            services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
            services.AddScoped<IIntegrationEventLogService, TIntegrationEventLogService>();

            var serviceBuilder = services.BuildServiceProvider();

            // check DaprClient is added
            if (serviceBuilder.GetService<DaprClient>() is null)
                throw new Exception("Please add DaprClient first.");

            // check AppConfig is configured
            if (serviceBuilder.GetService<IOptionsMonitor<AppConfig>>() is null)
                throw new Exception("Please configure the AppConfig options first.");

            return services;
        }
    }
}
