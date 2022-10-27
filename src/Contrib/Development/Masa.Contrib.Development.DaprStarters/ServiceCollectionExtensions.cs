// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprStarterCore(this IServiceCollection services, string sectionName = nameof(DaprOptions))
    {
        services.AddConfigure<DaprOptions>(sectionName);
        return services.AddDaprStarter();
    }

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

        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IDaprProcess, DaprProcess>();
        services.TryAddSingleton<IDaprProvider, DaprProvider>();
        services.TryAddSingleton<IProcessProvider, ProcessProvider>();
        return services;
    }

    private class DaprService
    {

    }
}
