// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprStarter(this IServiceCollection services)
        => services.AddDaprStarter(_ =>
        {
        });

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
        options.OnChange(daprOptions =>
        {
            daprProcess.Refresh(daprOptions);
        });
        daprProcess.Start(options.CurrentValue);
        CompleteDaprEnvironment(options.CurrentValue.DaprHttpPort, options.CurrentValue.DaprGrpcPort);
        return services;
    }

    private static void CompleteDaprEnvironment(ushort? daprHttpPort, ushort? daprGrpcPort)
    {
        if (daprHttpPort == null || daprGrpcPort == null)
            return;

        EnvironmentExtensions.TryAdd("DAPR_GRPC_PORT", daprGrpcPort.ToString, out _);
        EnvironmentExtensions.TryAdd("DAPR_HTTP_PORT", daprHttpPort.ToString, out _);
    }

    private class DaprService
    {

    }
}
