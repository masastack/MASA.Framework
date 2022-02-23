namespace MASA.Contrib.DDD.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventBus(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(DomainEventBusProvider)))
            return services;

        services.AddSingleton<DomainEventBusProvider>();

        var dispatcherOptions = new DispatcherOptions(services);
        options?.Invoke(dispatcherOptions);
        if (dispatcherOptions.Assemblies.Length == 0)
        {
            dispatcherOptions.Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }
        services.AddSingleton(typeof(IOptions<DispatcherOptions>), serviceProvider => Options.Create(dispatcherOptions));

        if (services.All(service => service.ServiceType != typeof(IEventBus)))
        {
            throw new Exception("Please add EventBus first.");
        }

        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            throw new Exception("Please add UoW first.");
        }

        if (services.All(service => service.ServiceType != typeof(IIntegrationEventBus)))
        {
            throw new Exception("Please add IntegrationEventBus first.");
        }

        services.CheckAggregateRootRepositories(dispatcherOptions.AllAggregateRootTypes);

        foreach (var domainServiceType in dispatcherOptions.AllDomainServiceTypes)
        {
            services.TryAddScoped(domainServiceType);
        }

        services.TryAddScoped<IDomainEventBus, DomainEventBus>();
        services.TryAddScoped<IDomainService, DomainService>();
        return services;
    }

    private static void CheckAggregateRootRepositories(this IServiceCollection services, List<Type> aggregateRootRepositoryTypes)
    {
        foreach (var aggregateRootRepositoryType in aggregateRootRepositoryTypes)
        {
            var serviceType = GetServiceRepositoryType(aggregateRootRepositoryType);
            if (services.All(service => service.ServiceType != serviceType))
            {
                throw new NotImplementedException($"The number of types of {serviceType.FullName} implementation class must be 1.");
            }
        }
    }

    private static Type GetServiceRepositoryType(Type entityType) => typeof(IRepository<>).MakeGenericType(entityType);

    private class DomainEventBusProvider
    {

    }
}
