namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public enum RetryType
{
    /// <summary>
    /// waits for the same period of time between each attempt. For example, it may retry the operation every 3 seconds.
    /// </summary>
    RegularIntervals,

    /// <summary>
    /// waits a short time before the first retry, and then incrementally increasing time between each subsequent retry.
    /// For example, it may retry the operation after 3 seconds, 7 seconds, 13 seconds, and so on.
    /// </summary>
    IncrementalIntervals
}
