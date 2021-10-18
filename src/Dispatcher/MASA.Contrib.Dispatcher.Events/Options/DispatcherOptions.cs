namespace MASA.Contrib.Dispatcher.Events.Options;

public class DispatcherOptions : IDispatcherOptions
{
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
            _allEventTypes = GetTypes(typeof(IEvent)).ToList();
        }
    }

    private IEnumerable<Type> Types { get; set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => type.IsAssignableFrom(t) && t.IsClass);

    public IEnumerable<Type> GetAllEventTypes() => _allEventTypes;

    public IServiceCollection Services { get; }

    private IEnumerable<Type> _allEventTypes;

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
