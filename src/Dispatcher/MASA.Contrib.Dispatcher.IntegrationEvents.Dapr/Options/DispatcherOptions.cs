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
                throw new ArgumentNullException(_pubSubName);
            }
            _pubSubName = value;
        }
    }

    public IServiceCollection Services { get; }

    private Assembly[] _assemblies = new Assembly[0];

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
            Types = _assemblies.SelectMany(assembly => assembly.GetTypes()).ToList();
            AllEventTypes = GetTypes(typeof(IEvent)).ToList();
        }
    }

    private List<Type> Types { get; set; }

    private List<Type> AllEventTypes { get; set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => type.IsAssignableFrom(t) && t.IsClass && t != typeof(IntegrationEvent));

    public IEnumerable<Type> GetAllEventTypes() => AllEventTypes;

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
