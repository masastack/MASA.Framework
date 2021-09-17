namespace MASA.Contrib.Dispatcher.Remoting.Dapr;

public static class ServiceCollectionExtensions
{
    internal static string GlobalDaprPubsubName { get; private set; }

    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(this IServiceCollection services)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        services.AddLogging();
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

    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(this IServiceCollection services, string daprPubsubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        GlobalDaprPubsubName = daprPubsubName;
        if (string.IsNullOrWhiteSpace(GlobalDaprPubsubName))
        {
            throw new ArgumentNullException(nameof(daprPubsubName));
        }
        return services.AddDaprEventBus<TIntegrationEventLogService>();
    }
}