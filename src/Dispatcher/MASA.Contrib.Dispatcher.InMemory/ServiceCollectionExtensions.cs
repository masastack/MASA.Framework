namespace MASA.Contrib.Dispatcher.InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
        => services.AddEventBus(ServiceLifetime.Scoped);

    public static IServiceCollection AddEventBus(this IServiceCollection services, params Assembly[] assemblies)
        => services.AddEventBus(ServiceLifetime.Scoped, assemblies);

    public static IServiceCollection AddEventBus(this IServiceCollection services, ServiceLifetime lifetime)
        => services.AddEventBus(lifetime, AppDomain.CurrentDomain.GetAssemblies());

    public static IServiceCollection AddEventBus(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        services.AddLogging();
        services.AddSingleton(new SagaDispatcher(services).Build(lifetime, assemblies));
        services.AddSingleton(new Dispatch.Dispatcher(services).Build(lifetime, assemblies));
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAdd(typeof(IEventBus), typeof(EventBus), lifetime);

        return services;
    }

    public static IServiceCollection AddTestEventBus(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        services.AddLogging();
        services.AddSingleton(new SagaDispatcher(services, true).Build(lifetime, assemblies));
        services.AddSingleton(new Dispatch.Dispatcher(services).Build(lifetime, assemblies));
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAdd(typeof(IEventBus), typeof(EventBus), lifetime);

        return services;
    }
}
