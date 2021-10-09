namespace MASA.Contrib.Dispatcher.Events;

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
            Types = Assemblies.SelectMany(assembly => assembly.GetTypes());
            AllEventTypes = GetTypes(typeof(IEvent));
        }
    }

    private IEnumerable<Type> Types { get; set; }

    private IEnumerable<Type> AllEventTypes { get; set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => type.IsAssignableFrom(t));

    public IEnumerable<Type> GetAllEventTypes() => AllEventTypes;

    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
