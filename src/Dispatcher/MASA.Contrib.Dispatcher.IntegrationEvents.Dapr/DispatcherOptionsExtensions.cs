namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDispatcherOptions options,
        string daprPubsubName = "pubsub",
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        if (options.Services == null)
        {
            throw new ArgumentNullException(nameof(options.Services));
        }

        options.Services.TryAddDaprEventBus<TIntegrationEventLogService>(builder, dispatcherOptions =>
        {
            dispatcherOptions.PubSubName = daprPubsubName;
        });
        return options;
    }
}
