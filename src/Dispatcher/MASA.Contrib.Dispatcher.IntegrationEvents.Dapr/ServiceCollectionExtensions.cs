using System.Reflection;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
       this IServiceCollection services,
       Action<DispatcherOptions>? options = null)
       where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddDaprEventBus<TIntegrationEventLogService>(null, options);

    internal static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DaprClientBuilder>? builder,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        var dispatcherOptions = new DispatcherOptions(services);
        options?.Invoke(dispatcherOptions);
        if (dispatcherOptions.Assemblies.Length == 0)
        {
            dispatcherOptions.Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }
        services.AddSingleton(typeof(IOptions<DispatcherOptions>), serviceProvider => Options.Create(dispatcherOptions));

        services.AddDaprClient(builder);
        services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
        services.AddScoped<IIntegrationEventLogService, TIntegrationEventLogService>();

        return services;
    }
}
