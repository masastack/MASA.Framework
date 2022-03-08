namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public Assembly[] Assemblies { get; }

    private string _pubSubName = "pubsub";

    public string PubSubName
    {
        get => _pubSubName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(PubSubName));

            _pubSubName = value;
        }
    }

    private int _localRetryTimes = 3;

    /// <summary>
    /// Local queue maximum number of retries
    /// </summary>
    public int LocalRetryTimes
    {
        get => _localRetryTimes;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(LocalRetryTimes));

            _localRetryTimes = value;
        }
    }

    private int _maxRetryTimes = 10;

    /// <summary>
    /// maximum number of retries
    /// Default is 10
    /// </summary>
    public int MaxRetryTimes
    {
        get => _maxRetryTimes;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(MaxRetryTimes));

            _maxRetryTimes = value;
        }
    }

    private int _failedRetryInterval = 60;

    /// <summary>
    /// The interval at which db polls for failure messages.
    /// Default is 60 seconds.
    /// unit: seconds
    /// </summary>
    public int FailedRetryInterval
    {
        get => _failedRetryInterval;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(FailedRetryInterval));

            _failedRetryInterval = value;
        }
    }

    private int _minimumRetryInterval = 60;

    /// <summary>
    /// Minimum execution retry interval
    /// Default is 60 seconds.
    /// </summary>
    public int MinimumRetryInterval
    {
        get => _minimumRetryInterval;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(MinimumRetryInterval));

            _minimumRetryInterval = value;
        }
    }

    private int _localFailedRetryInterval = 3;

    /// <summary>
    /// The interval at which the local queue is polled for failed messages.
    /// Local queue does not rebuild after service crash
    /// Default is 3 seconds.
    /// unit: seconds
    /// </summary>
    public int LocalFailedRetryInterval
    {
        get => _localFailedRetryInterval;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(LocalFailedRetryInterval));

            _localFailedRetryInterval = value;
        }
    }

    private int _retryBatchSize = 100;

    /// <summary>
    /// maximum number of retries per retry
    /// </summary>
    public int RetryBatchSize
    {
        get => _retryBatchSize;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(RetryBatchSize));

            _retryBatchSize = value;
        }
    }

    private int _cleaningLocalQueueExpireInterval = 60;

    /// <summary>
    /// Delete local queue expired event interval
    /// Default is 60 seconds
    /// unit: seconds
    /// </summary>
    public int CleaningLocalQueueExpireInterval
    {
        get => _cleaningLocalQueueExpireInterval;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(CleaningLocalQueueExpireInterval));

            _cleaningLocalQueueExpireInterval = value;
        }
    }

    private int _cleaningExpireInterval = 300;

    /// <summary>
    /// Delete expired event interval
    /// Default is 300 seconds.
    /// unit: seconds
    /// </summary>
    public int CleaningExpireInterval
    {
        get => _cleaningExpireInterval;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(CleaningExpireInterval));

            _cleaningExpireInterval = value;
        }
    }

    private long _publishedExpireTime = 24 * 3600;

    /// <summary>
    /// Expiration time, when the message status is successful and has expired, it will be deleted by the scheduled task
    /// Default: ( 24 * 3600 )s
    /// </summary>
    public long PublishedExpireTime
    {
        get => _publishedExpireTime;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(PublishedExpireTime));

            _publishedExpireTime = value;
        }
    }

    private int _deleteBatchCount = 1000;

    /// <summary>
    /// Bulk delete expired messages
    /// </summary>
    public int DeleteBatchCount
    {
        get => _deleteBatchCount;
        set
        {
            if (value <= 0)
                throw new ArgumentException("must be greater than 0", nameof(DeleteBatchCount));

            _deleteBatchCount = value;
        }
    }

    public Func<DateTime>? GetCurrentTime { get; set; }

    public List<Type> AllEventTypes { get; }

    private DispatcherOptions(IServiceCollection services) => Services = services;

    public DispatcherOptions(IServiceCollection services, Assembly[] assemblies)
        : this(services)
    {
        if (assemblies == null || assemblies.Length == 0)
            throw new ArgumentException(nameof(assemblies));

        Assemblies = assemblies;
        AllEventTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
    }
}
