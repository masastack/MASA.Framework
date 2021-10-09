using MASA.BuildingBlocks.DDD.Domain.Entities;

namespace MASA.Contribs.DDD.Domain;

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
            AllDomainServiceTypes = GetTypes(typeof(DomainService));
            AllAggregateRootTypes = GetTypes(typeof(IAggregateRoot)).Where(type => IsAggregateRootEntity(type));
        }
    }

    private bool IsAggregateRootEntity(Type type)
        => type.IsClass && !type.IsGenericType && !type.IsAbstract && type != typeof(AggregateRoot) && type != typeof(Entity) && typeof(IAggregateRoot).IsAssignableFrom(type);

    private IEnumerable<Type> Types { get; set; }

    public IEnumerable<Type> AllDomainServiceTypes { get; private set; }

    public IEnumerable<Type> AllAggregateRootTypes { get; private set; }

    private IEnumerable<Type> GetTypes(Type type) => Types.Where(t => type.IsAssignableFrom(t));

    public IServiceCollection Services { get; }

    public DispatcherOptions(IServiceCollection services) => Services = services;
}
