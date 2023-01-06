// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{

    #region Obsolete

    [Obsolete("Use AddIntegrationEventBus instead")]
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DaprIntegrationEventOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddDaprEventBus<TIntegrationEventLogService>(MasaApp.GetAssemblies(), options);

    [Obsolete("Use AddIntegrationEventBus instead")]
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DaprIntegrationEventOptions>? options = null,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddDaprEventBus<TIntegrationEventLogService>(assemblies, options, builder);

    internal static IServiceCollection TryAddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<DaprIntegrationEventOptions>? options,
        Action<DaprClientBuilder>? builder = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;

        services.AddSingleton<IntegrationEventBusProvider>();

        services.AddDaprClient(builder);

        return services.AddIntegrationEventBus<TIntegrationEventLogService>(assemblies, opt =>
        {
            DaprIntegrationEventOptions daprDispatcherOptions = new DaprIntegrationEventOptions(opt.Services, opt.Assemblies);
            options?.Invoke(daprDispatcherOptions);
            services.TryAddSingleton<IPublisher>(serviceProvider => new Publisher(serviceProvider, daprDispatcherOptions.PubSubName));

            daprDispatcherOptions.CopyTo(opt);
        });
    }

    private sealed class IntegrationEventBusProvider
    {
    }

    #endregion

}
