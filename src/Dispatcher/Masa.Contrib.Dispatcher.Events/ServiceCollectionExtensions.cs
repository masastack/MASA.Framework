namespace Masa.Contrib.Dispatcher.Events;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        => services.AddEventBus(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options = null)
        => services.AddEventBus(assemblies, ServiceLifetime.Scoped, options);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime,
        Action<DispatcherOptions>? options = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services, assemblies);
        options?.Invoke(dispatcherOptions);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        services.AddSingleton(new SagaDispatcher(services, dispatcherOptions.Assemblies).Build(lifetime));
        services.AddSingleton(new Internal.Dispatch.Dispatcher(services, dispatcherOptions.Assemblies).Build(lifetime));
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));
        return services;
    }

    public static IServiceCollection AddTestEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime,
        Action<DispatcherOptions>? options = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services, assemblies);
        options?.Invoke(dispatcherOptions);

        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
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
