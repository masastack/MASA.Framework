namespace Masa.Contrib.Ddd.Domain;

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
        => type.IsClass && !type.IsGenericType && !type.IsAbstract && type != typeof(AggregateRoot) && typeof(IAggregateRoot).IsAssignableFrom(type);

    private IEnumerable<Type> Types { get; set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => t.IsClass && type.IsAssignableFrom(t));

    internal List<Type> AllEventTypes { get; private set; }

    internal List<Type> AllDomainServiceTypes { get; private set; }

    internal List<Type> AllAggregateRootTypes { get; private set; }

    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
