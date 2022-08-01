// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<EventBusBuilder>? eventBusBuilder = null)
        => services.AddEventBus(AppDomain.CurrentDomain.GetAssemblies(), eventBusBuilder);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<EventBusBuilder>? eventBusBuilder = null)
        => services.AddEventBus(assemblies, ServiceLifetime.Scoped, eventBusBuilder);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime,
        Action<EventBusBuilder>? eventBusBuilder = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        var builder = new EventBusBuilder(services);
        eventBusBuilder?.Invoke(builder);

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services, assemblies);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        services.AddSingleton(new SagaDispatcher(services, assemblies).Build(lifetime));
        services.AddSingleton(new Dispatcher(services, assemblies).Build(lifetime));
        services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));
        return services;
    }

    public static IServiceCollection AddTestEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        ServiceLifetime lifetime,
        Action<EventBusBuilder>? eventBusBuilder = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        eventBusBuilder?.Invoke(new EventBusBuilder(services));

        DispatcherOptions dispatcherOptions = new DispatcherOptions(services, assemblies);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        services.AddSingleton(new SagaDispatcher(services, assemblies, true).Build(lifetime));
        services.AddSingleton(new Dispatcher(services, assemblies).Build(lifetime));
        services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));

        return services;
    }

    private class EventBusProvider
    {
    }
}
