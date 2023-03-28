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
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IDistributedCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualDistributedCacheClient>());

        services.AddTypeAlias(name);
    }

    public static void TryAddMultilevelCache(
        this IServiceCollection services,
        string name)
    {
        MasaApp.TrySetServiceCollection(services);

        services.TryAddTransient<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IMultilevelCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualMultilevelCacheClient>());

        services.AddTypeAlias(name);
    }

    private static void AddTypeAlias(
        this IServiceCollection services,
        string name)
    {
        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
        services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(name));
    }
}
