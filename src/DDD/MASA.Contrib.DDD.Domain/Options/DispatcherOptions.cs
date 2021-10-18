namespace MASA.Contrib.DDD.Domain;

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
            Types = _assemblies.SelectMany(assembly => assembly.GetTypes());
            AllEventTypes = GetTypes(typeof(IEvent)).ToList();
            AllDomainServiceTypes = GetTypes(typeof(DomainService)).ToList();
            AllAggregateRootTypes = GetTypes(typeof(IAggregateRoot)).Where(type => IsAggregateRootEntity(type)).ToList();
        }
    }

    private bool IsAggregateRootEntity(Type type)
        => type.IsClass && !type.IsGenericType && !type.IsAbstract && type != typeof(AggregateRoot) && type != typeof(Entity) && typeof(IAggregateRoot).IsAssignableFrom(type);

    private IEnumerable<Type> Types { get; set; }

    public List<Type> AllEventTypes { get; private set; }

    public List<Type> AllDomainServiceTypes { get; private set; }

    public List<Type> AllAggregateRootTypes { get; private set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => type.IsAssignableFrom(t) && t.IsClass);

    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
