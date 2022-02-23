namespace MASA.Contrib.Dispatcher.Events.Options;

public class DispatcherOptions : IDispatcherOptions
{
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

            UnitOfWorkRelation = AllEventTypes.ToDictionary(type => type, IsSupportUnitOfWork);
        }
    }

    private bool IsSupportUnitOfWork(Type eventType)
        => typeof(ITransaction).IsAssignableFrom(eventType) && !typeof(IDomainQuery<>).IsGenericInterfaceAssignableFrom(eventType);

    internal Dictionary<Type, bool> UnitOfWorkRelation { get; set; } = new();

    public IEnumerable<Type> AllEventTypes { get; private set; }

    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
