namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

/// <summary>
/// Local message retry logging
/// </summary>
public class IntegrationEventLogRetryItems
{
    public Guid Id { get; private set; }

    public Guid LogId { get; private set; }

    public DateTime CreationTime { get; private set; }

    public int RetryTimes { get; private set; }

    private IntegrationEventLogRetryItems()
    {
        Id = Guid.NewGuid();
        CreationTime = DateTime.UtcNow;
    }

    public IntegrationEventLogRetryItems(Guid logId, int retryTimes)
    {
        LogId = logId;
        RetryTimes = retryTimes;
    }
}

