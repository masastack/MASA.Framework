namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Internal;

internal class LocalQueueProcessor
{
    private readonly Dictionary<Guid, IntegrationEventLogItems> _retryEventLogs;

    public static ILogger<LocalQueueProcessor> Logger = default!;
    public static readonly LocalQueueProcessor Default = new();

    public LocalQueueProcessor() => _retryEventLogs = new();

    public static void SetLogger(IServiceCollection services)
    {
        Logger = services.BuildServiceProvider().GetRequiredService<ILogger<LocalQueueProcessor>>();
    }

    public void AddJobs(IntegrationEventLogItems items)
        => SafeMethods(() => _retryEventLogs.TryAdd(items.EventId, items), "add local queue jobs");

    public void RemoveJobs(Guid eventId)
        => SafeMethods(() => _retryEventLogs.Remove(eventId), "remove local queue jobs");

    public List<IntegrationEventLogItems> RetrieveEventLogsFailedToPublishAsync()
    {
        try
        {
            return _retryEventLogs.Select(x => x.Value).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "... getting local retry queue error");

            Task.Delay(TimeSpan.FromSeconds(2));
            return new List<IntegrationEventLogItems>();
        }
    }

    private void SafeMethods(Action action, string name)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "... {Name} failed", name);
            //ignore
        }
    }
}
