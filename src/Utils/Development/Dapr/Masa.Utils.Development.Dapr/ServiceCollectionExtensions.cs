// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public delegate void DaprEventHandler(string type, string data);

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprStarterCore(this IServiceCollection services, Action<DaprOptions>? action = null)
    {
        if (action != null)
            services.Configure(action);
        else
            services.Configure<DaprOptions>(_ =>
            {
            });
        return services.AddDaprStarter();
    }

    public static IServiceCollection AddDaprStarterCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DaprOptions>(configuration);
        return services.AddDaprStarter();
    }

    private static IServiceCollection AddDaprStarter(this IServiceCollection services)
    {
        if (services.Any(service => service.ImplementationType == typeof(DaprService)))
            return services;

        services.AddSingleton<DaprService>();

        services.TryAddSingleton(typeof(IDaprProcess), typeof(DaprProcess));
        services.TryAddSingleton(typeof(IDaprProvider), typeof(DaprProvider));
        services.TryAddSingleton(typeof(IProcessProvider), typeof(ProcessProvider));
        return services;
    }

    private class DaprService
    {

    }
}
