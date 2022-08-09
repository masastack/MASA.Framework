// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        => services.AddIntegrationEventBus(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options = null)
        => services.TryAddIntegrationEventBus(assemblies, options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddIntegrationEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddIntegrationEventBus<TIntegrationEventLogService>(assemblies, options);

    internal static IServiceCollection TryAddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        return services.TryAddIntegrationEventBus(assemblies, options, () =>
        {
            services.AddScoped<IIntegrationEventLogService, TIntegrationEventLogService>();
        });
    }

    internal static IServiceCollection TryAddIntegrationEventBus(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options,
        Action? action = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        var dispatcherOptions = new DispatcherOptions(services, assemblies);
        options?.Invoke(dispatcherOptions);

        services.TryAddSingleton(typeof(IOptions<DispatcherOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        LocalQueueProcessor.SetLogger(services);
        services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
        action?.Invoke();

        if (services.Any(d => d.ServiceType == typeof(IIntegrationEventLogService)))
        {
            services.AddSingleton<IProcessor, RetryByDataProcessor>();
            services.AddSingleton<IProcessor, RetryByLocalQueueProcessor>();
            services.AddSingleton<IProcessor, DeletePublishedExpireEventProcessor>();
            services.AddSingleton<IProcessor, DeleteLocalQueueExpiresProcessor>();
        }
        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();

        services.AddHostedService<IntegrationEventHostedService>();
        services.AddHostedService<InitializeMasaAppHostedService>();

        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IntegrationEventBus>>();
            logger?.LogDebug("UoW is not enabled or add delay, UoW is not used will affect 100% delivery of the message");
        }

        if (services.All(d => d.ServiceType != typeof(IPublisher)))
            throw new NotSupportedException($"{nameof(IPublisher)} has no implementing");

        return services;
    }

    private class IntegrationEventBusProvider
    {
    }
}
