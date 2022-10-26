// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
     public static IServiceCollection AddDaprStarter(this IServiceCollection services,
        string sectionName = nameof(DaprOptions),
        bool isDelay = true)
        => services.AddDaprStarter(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection(sectionName), isDelay);

    public static IServiceCollection AddDaprStarter(
        this IServiceCollection services,
        Action<DaprOptions> daprOptionAction,
        bool isDelay = true)
    {
        ArgumentNullException.ThrowIfNull(daprOptionAction, nameof(daprOptionAction));

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
        if (services.Any(service => service.ImplementationType == typeof(DaprService)))
            return services;

        services.AddSingleton<DaprService>();

        services.AddSingleton<IAppPortProvider, DefaultAppPortProvider>();
        action.Invoke();
        if (isDelay)
            return services.AddHostedService<DaprBackgroundService>();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();

        ArgumentNullException.ThrowIfNull(options.CurrentValue.AppPort, nameof(options.CurrentValue.AppPort));
        var daprProcess = serviceProvider.GetRequiredService<IDaprProcess>();
        daprProcess.Start();
        CompleteDaprEnvironment(options.CurrentValue.DaprHttpPort, options.CurrentValue.DaprGrpcPort);
        return services;
    }

    private static void CompleteDaprEnvironment(ushort? daprHttpPort, ushort? daprGrpcPort)
    {
        if (daprHttpPort == null || daprGrpcPort == null)
            return;

        EnvironmentUtils.TrySetEnvironmentVariable("DAPR_GRPC_PORT", daprGrpcPort.ToString());
        EnvironmentUtils.TrySetEnvironmentVariable("DAPR_HTTP_PORT", daprHttpPort.ToString());
    }

    private class DaprService
    {

    }
}
