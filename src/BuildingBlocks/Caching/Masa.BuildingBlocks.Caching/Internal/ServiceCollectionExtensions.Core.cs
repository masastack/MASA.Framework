// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.Distributed.StackExchangeRedis")]
[assembly: InternalsVisibleTo("Masa.Contrib.Caching.MultilevelCache")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

/// <summary>
/// It is only used for inheritance by Contrib implementation, and does not support the extension of IServiceCollection to avoid reference barriers
/// </summary>
internal static class ServiceCollectionExtensions
{
    public static void TryAddDistributedCache(
        this IServiceCollection services,
        string name)
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddTransient<IDistributedCacheClientFactory, DefaultDistributedCacheClientFactory>();
        services.TryAddTransient<IManualDistributedCacheClient>(serviceProvider =>
        {
            var cacheClient = serviceProvider.EnableIsolation() ?
                serviceProvider.GetRequiredService<ScopedService<IManualDistributedCacheClient>>().Service :
                serviceProvider.GetRequiredService<SingletonService<IManualDistributedCacheClient>>().Service;
            return new DefaultDistributedCacheClient(cacheClient);
        });
        services.TryAddTransient<IDistributedCacheClient>(serviceProvider
            => serviceProvider.GetRequiredService<IManualDistributedCacheClient>());

        services.AddCaching();
        services.AddTypeAlias(name);
    }

    public static void TryAddMultilevelCache(
        this IServiceCollection services,
        string name)
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddTransient<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        services.TryAddTransient<IManualMultilevelCacheClient>(serviceProvider =>
        {
            var cacheClient = serviceProvider.GetRequiredService<IManualMultilevelCacheClient>();
            return new DefaultMultilevelCacheClient(cacheClient);
        });
        services.TryAddTransient<IMultilevelCacheClient>(serviceProvider
            => serviceProvider.GetRequiredService<IManualMultilevelCacheClient>());

        services.AddCaching();
        services.AddTypeAlias(name);
    }

    private static void AddTypeAlias(
        this IServiceCollection services,
        string name)
    {
        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
        services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(name));
    }

    private static void AddCaching(this IServiceCollection services)
    {
        services.TryAddScoped(serviceProvider => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        services.TryAddScoped(serviceProvider => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
    }
}
