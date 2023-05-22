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

        services.TryAddEnumerable(new ServiceDescriptor(typeof(IEventMiddleware<>), typeof(TransactionEventMiddleware<>),
            ServiceLifetime.Transient));

        var builder = new EventBusBuilder(services);
        eventBusBuilder?.Invoke(builder);

        MasaArgumentException.ThrowIfNullOrEmptyCollection(assemblies);

        var assemblyArray = assemblies.Distinct().ToArray();

        // var dispatcherOptions = new DispatcherOptions(services, assemblyArray);
        // services.AddSingleton(typeof(IOptions<DispatcherOptions>),
        //     _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
        // services.AddSingleton(new SagaDispatcher(services, assemblyArray).Build(lifetime));
        // services.AddSingleton(new Dispatcher(services, assemblyArray).Build(lifetime));


        #region 构建关系

        var dispatchNetworkBuilder = new DispatchRelationNetworkBuilder();
        var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
        dispatchNetworkBuilder.Add(new DefaultDispatchNetworkProvider(assemblyArray, loggerFactory));
        dispatchNetworkBuilder.Add(new DefaultSagaDispatchNetworkProvider(assemblyArray, loggerFactory));
        var dispatchNetworkRoot = dispatchNetworkBuilder.Build();
        services.AddSingleton<IDispatchNetworkRoot>(_ => dispatchNetworkRoot);

        var serviceTypes =
            DispatchNetworkUtils.GetServiceTypes(dispatchNetworkRoot.DispatchNetwork.SelectMany(item => item.Value).ToList());
        foreach (var serviceType in serviceTypes)
        {
            services.TryAdd(new ServiceDescriptor(serviceType, serviceType, lifetime));
        }

        var dispatcherOptions = new DispatcherOptions(services, assemblyArray);
        services.AddSingleton<IOptions<DispatcherOptions>>(_ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        #endregion

        services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
        services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
        services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();

        services.TryAdd(new ServiceDescriptor(typeof(LocalEventBusPublisher), typeof(LocalEventBusPublisher), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(ILocalEventBus), typeof(LocalEventBus), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IEventBus), serviceProvider =>
        {
            var eventBus = new EventBus(
                new Lazy<ILocalEventBus>(serviceProvider.GetRequiredService<ILocalEventBus>),
                new Lazy<IIntegrationEventBus?>(serviceProvider.GetService<IIntegrationEventBus>));
            return eventBus;
        }, lifetime));

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    // public static IServiceCollection AddTestEventBus(
    //     this IServiceCollection services,
    //     IEnumerable<Assembly> assemblies,
    //     ServiceLifetime lifetime,
    //     Action<EventBusBuilder>? eventBusBuilder = null)
    // {
    //     if (services.Any(service => service.ImplementationType == typeof(EventBusProvider)))
    //         return services;
    //
    //     services.AddSingleton<EventBusProvider>();
    //
    //     eventBusBuilder?.Invoke(new EventBusBuilder(services));
    //
    //     MasaArgumentException.ThrowIfNullOrEmptyCollection(assemblies);
    //
    //     var assemblyArray = assemblies.Distinct().ToArray();
    //     var dispatcherOptions = new DispatcherOptions(services, assemblyArray);
    //     services.AddSingleton(typeof(IOptions<DispatcherOptions>),
    //         _ => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));
    //     services.AddSingleton(new SagaDispatcher(services, assemblyArray, true).Build(lifetime));
    //     services.AddSingleton(new Dispatcher(services, assemblyArray).Build(lifetime));
    //     services.TryAddSingleton<IExceptionStrategyProvider, DefaultExceptionStrategyProvider>();
    //     services.TryAdd(typeof(IExecutionStrategy), typeof(ExecutionStrategy), ServiceLifetime.Singleton);
    //     services.TryAddScoped<IInitializeServiceProvider, InitializeServiceProvider>();
    //     services.AddTransient(typeof(IEventMiddleware<>), typeof(TransactionEventMiddleware<>));
    //     services.AddScoped(typeof(IEventBus), typeof(LocalEventBus));
    //
    //     return services;
    // }

    private sealed class EventBusProvider
    {
    }
}
