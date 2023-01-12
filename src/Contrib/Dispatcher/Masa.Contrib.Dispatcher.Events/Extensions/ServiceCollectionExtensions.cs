// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        Action<EventBusBuilder>? eventBusBuilder = null)
        => services.AddEventBus(MasaApp.GetAssemblies(), eventBusBuilder);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<EventBusBuilder>? eventBusBuilder = null)
        => services.AddEventBus(assemblies, ServiceLifetime.Scoped, eventBusBuilder);

    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        Action<EventBusBuilder>? eventBusBuilder = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        var builder = new EventBusBuilder(services);
        eventBusBuilder?.Invoke(builder);

        MasaArgumentException.ThrowIfNullOrEmptyCollection(assemblies);

        var assemblyArray = assemblies.Distinct().ToArray();
        var dispatcherOptions = new DispatcherOptions(services, assemblyArray);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        services.AddSingleton(new SagaDispatcher(services, assemblyArray).Build(lifetime));
        services.AddSingleton(new Dispatcher(services, assemblyArray).Build(lifetime));
        services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();
        services.TryAddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static IServiceCollection AddTestEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime,
        Action<EventBusBuilder>? eventBusBuilder = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
            return services;

        services.AddSingleton<EventBusProvider>();

        eventBusBuilder?.Invoke(new EventBusBuilder(services));

        MasaArgumentException.ThrowIfNullOrEmptyCollection(assemblies);

        var assemblyArray = assemblies.Distinct().ToArray();
        var dispatcherOptions = new DispatcherOptions(services, assemblyArray);
        services.AddSingleton(typeof(IOptions<DispatcherOptions>),
            _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        services.AddSingleton(new SagaDispatcher(services, assemblyArray, true).Build(lifetime));
        services.AddSingleton(new Dispatcher(services, assemblyArray).Build(lifetime));
        services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();
        services.AddTransient(typeof(IMiddleware<>), typeof(TransactionMiddleware<>));
        services.AddScoped(typeof(IEventBus), typeof(EventBus));

        return services;
    }

    private sealed class EventBusProvider
    {
    }
}
