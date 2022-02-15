namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Internal;

internal class LocalQueueProcessor
{
    private readonly ConcurrentDictionary<Guid, IntegrationEventLogItems> _retryEventLogs;

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
        => SafeMethods(() => _retryEventLogs.TryRemove(eventId, out _), "remove local queue jobs");

    public void RetryJobs(Guid eventId)
    {
        if (_retryEventLogs.TryGetValue(eventId, out IntegrationEventLogItems? item))
        {
            item.Retry();
        }
    }

    public bool IsExist(Guid eventId)
        => _retryEventLogs.ContainsKey(eventId);

    public void DeleteAsync(int maxRetryTimes)
    {
        var eventLogItems = _retryEventLogs.Values.Where(log => log.RetryCount >= maxRetryTimes - 1).ToList();
        eventLogItems.ForEach(item =>
        {
            _retryEventLogs.TryRemove(item.EventId, out _);
        });
    }

    public List<IntegrationEventLogItems> RetrieveEventLogsFailedToPublishAsync(int maxRetryTimes)
    {
        try
        {
            return _retryEventLogs
                .Select(item => item.Value)
                .Where(log => log.RetryCount < maxRetryTimes)
                .OrderBy(log => log.RetryCount)
                .ThenBy(log => log.CreationTime)
                .ToList();
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
