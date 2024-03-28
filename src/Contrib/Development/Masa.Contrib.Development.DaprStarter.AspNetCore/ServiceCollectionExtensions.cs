// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprStarter(this IServiceCollection services,
        string sectionName = nameof(DaprOptions),
        bool isDelay = true)
    {
        return services.AddDaprStarter(() =>
        {
            services.AddDaprStarterCore(sectionName);
        }, isDelay);
    }

    public static IServiceCollection AddDaprStarter(
        this IServiceCollection services,
        Action<DaprOptions> daprOptionAction,
        bool isDelay = true)
    {
        ArgumentNullException.ThrowIfNull(daprOptionAction);

        return services.AddDaprStarter(() =>
        {
            services.AddDaprStarterCore(daprOptionAction);
        }, isDelay);
    }

    public static IServiceCollection AddDaprStarter(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDelay = true)
    {
        return services.AddDaprStarter(() =>
        {
            services.AddDaprStarterCore(configuration);
        }, isDelay);
    }

    private static IServiceCollection AddDaprStarter(this IServiceCollection services, Action action, bool isDelay = true)
    {

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(DaprService)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(DaprService)))
            return services;
#endif

        services.AddSingleton<DaprService>();

        services.TryAddSingleton<IAppPortProvider, DefaultAppPortProvider>();
        services.TryAddSingleton<IAvailabilityPortProvider, DefaultAvailabilityPortProvider>();
        action.Invoke();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        if (isDelay) return services.AddHostedService<DaprBackgroundService>();

        PortUtils.CheckCompletionPort(options.CurrentValue, serviceProvider);

        ArgumentNullException.ThrowIfNull(options.CurrentValue.AppPort);
        var daprProcess = serviceProvider.GetRequiredService<IDaprProcess>();
        daprProcess.Start();
        return services;
    }




    private sealed class DaprService
    {
    }
}
