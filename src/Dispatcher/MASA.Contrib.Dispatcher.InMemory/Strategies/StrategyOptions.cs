namespace MASA.Contrib.Dispatcher.InMemory.Strategies;

public class StrategyOptions
{
    /// <summary>
    /// The maximum number of retry attempts.
    /// </summary>
    public int MaxRetryCount { get; set; }

    public FailureLevels FailureLevels { get; set; }

    public bool IsRetry(int retryTimes) => retryTimes <= MaxRetryCount;

    public void SetStrategy(EventHandlerAttribute dispatchHandler)
    {
        MaxRetryCount = dispatchHandler.ActualRetryTimes; ;
        FailureLevels = dispatchHandler.FailureLevels;
    }
}
