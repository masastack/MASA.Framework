// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    #region Obsolete

    [Obsolete("Use AddIntegrationEventBus instead")]
    public static IServiceCollection AddDaprEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<DispatcherOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddDaprEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    [Obsolete("Use AddIntegrationEventBus instead")]
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

        services.AddDaprClient(builder);

        return services.AddIntegrationEventBus<TIntegrationEventLogService>(assemblies, opt =>
        {
            DispatcherOptions daprDispatcherOptions = new DispatcherOptions(opt.Services, opt.Assemblies);
            options?.Invoke(daprDispatcherOptions);
            services.TryAddSingleton<IPublisher>(serviceProvider=> new Publisher(serviceProvider,daprDispatcherOptions.PubSubName));

            daprDispatcherOptions.CopyTo(opt);
        });
    }

    private class IntegrationEventBusProvider
    {
    }

    #endregion
}
