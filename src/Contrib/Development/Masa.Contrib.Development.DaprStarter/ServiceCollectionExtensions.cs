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

    public static IServiceCollection AddDaprStarterCore(this IServiceCollection services, Action<DaprOptions> action)
    {
        services.Configure(action);
        return services.AddDaprStarter();
    }

    public static IServiceCollection AddDaprStarterCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DaprOptions>(configuration);
        return services.AddDaprStarter();
    }

    private static IServiceCollection AddDaprStarter(this IServiceCollection services)
    {

#if (NET8_0 || NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(DaprService)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(DaprService)))
            return services;
#endif

        services.AddSingleton<DaprService>();

        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IDaprProcess, DaprProcess>();
        services.TryAddSingleton<IDaprProvider, DefaultDaprProvider>();
        services.TryAddSingleton<IDaprProcessProvider, DaprProcessProvider>();
        services.TryAddSingleton<IProcessProvider, ProcessProvider>();
        services.TryAddSingleton<IDaprEnvironmentProvider, DaprEnvironmentProvider>();
        return services;
    }

    private sealed class DaprService
    {
    }
}
