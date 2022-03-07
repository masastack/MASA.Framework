namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class DispatcherOptionsExtensions
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public static IDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDispatcherOptions options,
        string daprPubsubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => options.UseDaprEventBus<TIntegrationEventLogService>(daprPubsubName, null);

    public static IDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDispatcherOptions options,
        string daprPubsubName,
        Action<DaprClientBuilder>? builder)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        return options.UseDaprEventBus<TIntegrationEventLogService>(builder, dispatcherOptions =>
        {
            dispatcherOptions.PubSubName = daprPubsubName;
        });
    }

    public static IDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDispatcherOptions options,
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
