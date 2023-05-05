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
        return services.AddDaprStarter(() => { services.AddDaprStarterCore(sectionName); }, isDelay);
    }

    public static IServiceCollection AddDaprStarter(
        this IServiceCollection services,
        Action<DaprOptions> daprOptionAction,
        bool isDelay = true)
    {
        ArgumentNullException.ThrowIfNull(daprOptionAction);

        return services.AddDaprStarter(() => { services.AddDaprStarterCore(daprOptionAction); }, isDelay);
    }

    public static IServiceCollection AddDaprStarter(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDelay = true)
    {
        return services.AddDaprStarter(() => { services.AddDaprStarterCore(configuration); }, isDelay);
    }

    private static IServiceCollection AddDaprStarter(this IServiceCollection services, Action action, bool isDelay = true)
    {
        if (services.Any(service => service.ImplementationType == typeof(DaprService)))
            return services;

        services.AddSingleton<DaprService>();

        services.TryAddSingleton<IAppPortProvider, DefaultAppPortProvider>();
        services.TryAddSingleton<IAvailabilityPortProvider, DefaultAvailabilityPortProvider>();
        action.Invoke();

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<DaprOptions>>();
        CheckCompletionHttPortAndGrpcPort(options.CurrentValue, serviceProvider);

        if (isDelay) return services.AddHostedService<DaprBackgroundService>();

        ArgumentNullException.ThrowIfNull(options.CurrentValue.AppPort);
        var daprProcess = serviceProvider.GetRequiredService<IDaprProcess>();
        daprProcess.Start();
        return services;
    }

    private static void CheckCompletionHttPortAndGrpcPort(DaprOptions daprOptions, IServiceProvider serviceProvider)
    {
        var daprEnvironmentProvider = serviceProvider.GetRequiredService<IDaprEnvironmentProvider>();

        daprOptions.DaprHttpPort ??= daprEnvironmentProvider.GetHttpPort();
        daprOptions.DaprGrpcPort ??= daprEnvironmentProvider.GetGrpcPort();

        var httpPortStatus = IsAvailablePort(daprOptions.DaprHttpPort);
        var gRpcPortStatus = IsAvailablePort(daprOptions.DaprGrpcPort);
        var httpPortByAvailability = daprOptions.DaprHttpPort;
        var gRpcPortByAvailability = daprOptions.DaprGrpcPort;
        if (!httpPortStatus || !gRpcPortStatus)
        {
            var reservedPorts = new List<int>();
            var availabilityPortProvider = serviceProvider.GetRequiredService<IAvailabilityPortProvider>();

            if (!httpPortStatus && !gRpcPortStatus)
            {
                //httpPort and grpcPort are not available
                httpPortByAvailability = availabilityPortProvider.GetAvailablePort(3500, reservedPorts);
                if (httpPortByAvailability != null) reservedPorts.Add(httpPortByAvailability.Value);
                gRpcPortByAvailability = availabilityPortProvider.GetAvailablePort(50001, reservedPorts);
            }
            else if (!httpPortStatus)
            {
                //httpPort is not available
                reservedPorts.Add(gRpcPortByAvailability!.Value);
                httpPortByAvailability = availabilityPortProvider.GetAvailablePort(3500);
            }
            else if (!gRpcPortStatus)
            {
                //gRpcPort is not available
                reservedPorts.Add(httpPortByAvailability!.Value);
                gRpcPortByAvailability = availabilityPortProvider.GetAvailablePort(50001, reservedPorts);
            }
        }

        daprEnvironmentProvider.TrySetHttpPort(httpPortByAvailability);
        daprEnvironmentProvider.TrySetGrpcPort(gRpcPortByAvailability);

        // Environment variables need to be improved
        bool IsAvailablePort([NotNullWhen(true)] ushort? port)
        {
            return port is > 0;
        }
    }


    private sealed class DaprService
    {
    }
}
