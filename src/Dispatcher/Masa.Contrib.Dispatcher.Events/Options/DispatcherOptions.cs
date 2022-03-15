namespace Masa.Contrib.Dispatcher.Events.Options;

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public Assembly[] Assemblies { get; }

    private bool IsSupportUnitOfWork(Type eventType)
        => typeof(ITransaction).IsAssignableFrom(eventType) && !typeof(IDomainQuery<>).IsGenericInterfaceAssignableFrom(eventType);

    internal Dictionary<Type, bool> UnitOfWorkRelation { get; } = new();

    public IEnumerable<Type> AllEventTypes { get; }

    private DispatcherOptions(IServiceCollection services) => Services = services;

    public DispatcherOptions(IServiceCollection services, Assembly[] assemblies)
        : this(services)
    {
        if (assemblies == null || assemblies.Length == 0)
            throw new ArgumentException(nameof(assemblies));

        Assemblies = assemblies;
        AllEventTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
        UnitOfWorkRelation = AllEventTypes.ToDictionary(type => type, IsSupportUnitOfWork);
    }

    public DispatcherOptions UseMiddleware(Type middleware, ServiceLifetime middlewareLifetime = ServiceLifetime.Scoped)
    {
        var descriptor = new ServiceDescriptor(typeof(IMiddleware<>), middleware, middlewareLifetime);
        Services.TryAddEnumerable(descriptor);
        return this;
    }
}
