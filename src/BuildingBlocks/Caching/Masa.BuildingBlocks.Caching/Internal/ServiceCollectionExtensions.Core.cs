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
    public static void TryAddDistributedCache(this IServiceCollection services)
    {
        MasaApp.TrySetServiceCollection(services);
        services.AddServiceFactory();

        services.TryAddTransient<IDistributedCacheClientFactory, DefaultDistributedCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IDistributedCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualDistributedCacheClient>());

        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
    }

    public static void TryAddMultilevelCache(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, IManualMultilevelCacheClient> func)
    {
        services.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IManualMultilevelCacheClient>(name, func.Invoke);
            options.Options.Add(cacheRelationOptions);
        });

        services.TryAddTransient<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IMultilevelCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualMultilevelCacheClient>());

        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
        services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(name));

        services.TryAddDistributedCache();
    }
}
