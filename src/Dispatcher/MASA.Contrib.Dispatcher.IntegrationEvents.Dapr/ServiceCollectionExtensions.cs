namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddDaprEventBus<TIntegrationEventLogService>(null, options);

    internal static IServiceCollection TryAddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DaprClientBuilder>? builder,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        var dispatcherOptions = new DispatcherOptions(services);
        options?.Invoke(dispatcherOptions);

        if (dispatcherOptions.Assemblies.Length == 0)
            dispatcherOptions.Assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.TryAddSingleton(typeof(IOptions<DispatcherOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        services.AddLogging();
        LocalQueueProcessor.SetLogger(services);
        services.AddDaprClient(builder);
        services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
        services.AddScoped<IIntegrationEventLogService, TIntegrationEventLogService>();
        services.AddSingleton<IProcessor, RetryByDataProcessor>();
        services.AddSingleton<IProcessor, RetryByLocalQueueProcessor>();
        services.AddSingleton<IProcessor, DeletePublishedExpireEventProcessor>();
        services.AddSingleton<IProcessor, DeleteLocalQueueExpiresProcessor>();
        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();
        services.AddHostedService<IntegrationEventHostedService>();
        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IntegrationEventBus>>();
            logger?.LogWarning("UoW is not enabled, local messages will not be integrated");
        }

        return services;
    }

    private class IntegrationEventBusProvider
    {
    }
}
