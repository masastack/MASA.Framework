// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddDaprEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options = null,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddDaprEventBus<TIntegrationEventLogService>(assemblies, options, builder);

    internal static IServiceCollection TryAddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DispatcherOptions>? options,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        services.TryAddSingleton<IPublisher, Publisher>();
        services.AddDaprClient(builder);

        return services.AddIntegrationEventBus<TIntegrationEventLogService>(assemblies, opt =>
        {
            DispatcherOptions daprDispatcherOptions = new DispatcherOptions(opt.Services, opt.Assemblies);
            options?.Invoke(daprDispatcherOptions);

            services.TryAddSingleton(typeof(IOptions<DispatcherOptions>),
                serviceProvider => Microsoft.Extensions.Options.Options.Create(daprDispatcherOptions));

            daprDispatcherOptions.CopyTo(opt);
        });
    }

    private class IntegrationEventBusProvider
    {
    }
}
