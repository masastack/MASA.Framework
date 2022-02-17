namespace MASA.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;

public class DispatcherOptions : IDispatcherOptions
{
    private string _pubSubName = "pubsub";

    public string PubSubName
    {
        get => _pubSubName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(_pubSubName));
            }

            _pubSubName = value;
        }
    }

    /// <summary>
    /// maximum number of retries
    /// Local queue maximum number of retries
    /// </summary>
    public int LocalRetryTimes { get; set; } = 3;

    /// <summary>
    /// maximum number of retries
    /// Default is 10
    /// </summary>
    public int MaxRetryTimes { get; set; } = 10;

    /// <summary>
    /// The interval at which db polls for failure messages.
    /// Default is 60 seconds.
    /// </summary>
    public int FailedRetryInterval { get; set; } = 60;

    /// <summary>
    /// The interval at which the local queue is polled for failed messages.
    /// Local queue does not rebuild after service crash
    /// Default is 3 seconds.
    /// </summary>
    public int LocalFailedRetryInterval { get; set; } = 3;

    /// <summary>
    /// The size of a single event to be retried
    /// </summary>
    public int RetryBatchSize { get; set; } = 100;

    /// <summary>
    /// Delete local queue expired event interval
    /// Default is 60 seconds.
    /// </summary>
    public int CleaningLocalQueueExpireInterval { get; set; } = 60;

    /// <summary>
    /// Delete expired event interval
    /// Default is 300 seconds.
    /// </summary>
    public int CleaningExpireInterval { get; set; } = 300;

    /// <summary>
    /// Expiration time, when the message status is successful and has expired, it will be deleted by the scheduled task
    /// Default: ( 24 * 3600 )s
    /// </summary>
    public long PublishedExpireTime { get; set; } = 24 * 3600;

    /// <summary>
    /// Bulk delete expired messages
    /// </summary>
    public int DeleteBatchCount { get; set; } = 1000;

    public Func<DateTime>? GetCurrentTime { get; set; } = null;

    public IServiceCollection Services { get; }

    private Assembly[] _assemblies = Array.Empty<Assembly>();

    public Assembly[] Assemblies
    {
        get => _assemblies;
        set
        {
            _assemblies = value;
            if (_assemblies == null || _assemblies.Length == 0)
            {
                throw new ArgumentNullException(nameof(_assemblies));
            }

            AllEventTypes = _assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
                .ToList();
        }
    }

    public List<Type> AllEventTypes { get; private set; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
