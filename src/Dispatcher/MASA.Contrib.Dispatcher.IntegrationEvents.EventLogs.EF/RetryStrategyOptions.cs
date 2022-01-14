namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class RetryStrategyOptions
{
    public RetryType RetryType { get; private set; }

    public int MaxRetryTimes { get; private set; }

    /// <summary>
    /// When the type is regular intervals, this value should be of type int, example: 3
    /// When the type is incremental interval, the format is: 1 3 7
    /// unit: s
    /// </summary>
    public string Rule { get; private set; }

    public RetryStrategyOptions()
    {
        UseRegularIntervals(30, 3);
    }

    public RetryStrategyOptions(RetryType retryType, int maxRetryTimes, string rule)
    {
        RetryType = retryType;
        MaxRetryTimes = maxRetryTimes;
        Rule = rule;
    }

    public RetryStrategyOptions UseRegularIntervals(int intervals, int maxRetryTimes = 3)
        => new RetryStrategyOptions(RetryType.RegularIntervals, maxRetryTimes, intervals.ToString());

    public RetryStrategyOptions UseIncrementalIntervals(int maxRetryTimes = 3, params int[] intervals)
    {
        if (maxRetryTimes != intervals.Length)
            throw new ArgumentException("Invalid retry rule");

        return new RetryStrategyOptions(RetryType.IncrementalIntervals, maxRetryTimes, string.Join(" ", intervals));
    }
}
