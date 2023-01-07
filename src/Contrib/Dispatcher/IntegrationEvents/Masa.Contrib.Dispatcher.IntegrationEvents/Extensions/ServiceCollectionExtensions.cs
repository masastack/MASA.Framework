// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        Action<IntegrationEventOptions>? options = null)
        => services.AddIntegrationEventBus(MasaApp.GetAssemblies(), options);

    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options = null)
        => services.TryAddIntegrationEventBus(assemblies, options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<IntegrationEventOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddIntegrationEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddIntegrationEventBus<TIntegrationEventLogService>(assemblies, options);

    internal static IServiceCollection TryAddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddIntegrationEventBus(
            assemblies,
            options,
            services.TryAddScoped<IIntegrationEventLogService, TIntegrationEventLogService>);

    internal static IServiceCollection TryAddIntegrationEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options,
        Action? action = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        MasaArgumentException.ThrowIfNull(assemblies);
        var dispatcherOptions = new IntegrationEventOptions(services, assemblies.Distinct().ToArray());
        options?.Invoke(dispatcherOptions);

        services.TryAddSingleton(typeof(IOptions<IntegrationEventOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        LocalQueueProcessor.SetLogger(services);
        services.AddScoped<IIntegrationEventBus, IntegrationEventBus>();
        action?.Invoke();

        if (services.Any(d => d.ServiceType == typeof(IIntegrationEventLogService)))
        {
            services.AddSingleton<IProcessor, RetryByDataProcessor>();
            services.AddSingleton<IProcessor, RetryByLocalQueueProcessor>();
            services.AddSingleton<IProcessor, SendByDataProcessor>();
            services.AddSingleton<IProcessor, DeletePublishedExpireEventProcessor>();
            services.AddSingleton<IProcessor, DeleteLocalQueueExpiresProcessor>();
        }
        else
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IntegrationEventBus>>();
            logger?.LogWarning("The local message table is not used correctly, it will cause integration events not to be sent normally");
        }
        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();

        services.AddHostedService<IntegrationEventHostedService>();

        if (services.All(service => service.ServiceType != typeof(IUnitOfWork)))
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IntegrationEventBus>>();
            logger?.LogDebug("UoW is not enabled or add delay, UoW is not used will affect 100% delivery of the message");
        }

        if (services.All(d => d.ServiceType != typeof(IPublisher)))
            throw new NotSupportedException($"{nameof(IPublisher)} has no implementing");

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class IntegrationEventBusProvider
    {
    }
}
