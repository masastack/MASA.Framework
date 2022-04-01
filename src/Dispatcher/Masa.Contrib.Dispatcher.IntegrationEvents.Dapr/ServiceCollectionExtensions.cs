namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddDaprEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options = null,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddDaprEventBus<TIntegrationEventLogService>(assemblies, options, builder);

    internal static IServiceCollection TryAddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        var dispatcherOptions = new DispatcherOptions(services, assemblies);
        options?.Invoke(dispatcherOptions);

        services.TryAddSingleton(typeof(IOptions<DispatcherOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

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
            logger?.LogDebug("UoW is not enabled or add delay, UoW is not used will affect 100% delivery of the message");
        }

        return services;
    }

    private class IntegrationEventBusProvider
    {
    }
}
