namespace Masa.Contrib.Dispatcher.Events;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        => services.AddEventBus(ServiceLifetime.Scoped, options);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Action<DispatcherOptions>? options = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider))) return services;
        services.AddSingleton<EventBusProvider>();

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services);
        options?.Invoke(dispatcherOptions);
        if (dispatcherOptions.Assemblies.Length == 0)
        {
            dispatcherOptions.Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }
        services.AddSingleton(typeof(IOptions<DispatcherOptions>), serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        services.AddSingleton(new SagaDispatcher(services, dispatcherOptions.Assemblies).Build(lifetime));
        services.AddSingleton(new Internal.Dispatch.Dispatcher(services, dispatcherOptions.Assemblies).Build(lifetime));
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));
        return services;
    }

    public static IServiceCollection AddTestEventBus(this IServiceCollection services, ServiceLifetime lifetime,
        Action<DispatcherOptions>? options = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider))) return services;
        services.AddSingleton<EventBusProvider>();

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services);
        options?.Invoke(dispatcherOptions);
        if (dispatcherOptions.Assemblies.Length == 0)
        {
            dispatcherOptions.Assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        services.AddSingleton(typeof(IOptions<DispatcherOptions>), serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        services.AddSingleton(new SagaDispatcher(services, dispatcherOptions.Assemblies, true).Build(lifetime));
        services.AddSingleton(new Internal.Dispatch.Dispatcher(services, dispatcherOptions.Assemblies).Build(lifetime));
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));

        return services;
    }

    private class EventBusProvider
    {

    }
}
