namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class DispatcherOptionsExtensions
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions options,
        string daprPubSubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => options.UseDaprEventBus<TIntegrationEventLogService>(daprPubSubName, null);

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions options,
        string daprPubSubName,
        Action<DaprClientBuilder>? builder)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        return options.UseDaprEventBus<TIntegrationEventLogService>(builder, dispatcherOptions =>
        {
            dispatcherOptions.PubSubName = daprPubSubName;
        });
    }

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions options,
        Action<DaprClientBuilder>? builder = null,
        Action<DispatcherOptions>? action = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        ArgumentNullException.ThrowIfNull(options.Services, nameof(options.Services));

        options.Services.TryAddDaprEventBus<TIntegrationEventLogService>(options.Assemblies, builder, dispatcherOptions =>
        {
            dispatcherOptions.PubSubName = DAPR_PUBSUB_NAME;
            action?.Invoke(dispatcherOptions);
        });
        return options;
    }
}
