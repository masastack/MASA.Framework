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
    /// When there is no message to retry, the thread sleep time
    /// default: 30s
    /// </summary>
    public int IdleTime { get; set; } = 30;

    /// <summary>
    /// The size of a single event to be retried
    /// default: 100
    /// </summary>
    public int RetryDepth { get; set; } = 100;

    public bool IsRetry { get; set; } = true;

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
