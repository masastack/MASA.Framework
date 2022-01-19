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
    /// default: 100
    /// </summary>
    public int RetryBatchSize { get; set; } = 100;

    /// <summary>
    /// Delete expired event interval
    /// Default is 300 seconds.
    /// </summary>
    public int CleaningExpireInterval { get; set; }

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
