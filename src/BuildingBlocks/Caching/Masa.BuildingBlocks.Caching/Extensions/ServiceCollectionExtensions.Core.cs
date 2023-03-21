// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching.Extensions;

/// <summary>
/// It is only used for inheritance by Contrib implementation, and does not support the extension of IServiceCollection to avoid reference barriers
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddDistributedCacheCore(IServiceCollection services, string name)
    {
        MasaApp.TrySetServiceCollection(services);
        services.AddServiceFactory();

        services.TryAddSingleton<IDistributedCacheClientFactory, DefaultDistributedCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IDistributedCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualDistributedCacheClient>());

        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
        services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(name));
        return services;
    }

    public static IServiceCollection TryAddMultilevelCacheCore(IServiceCollection services, string name)
    {
        services.TryAddSingleton<IMultilevelCacheClientFactory, DefaultMultilevelCacheClientFactory>();
        services.TryAddSingleton(serviceProvider
            => serviceProvider.GetRequiredService<IMultilevelCacheClientFactory>().Create());
        services.TryAddSingleton(typeof(IMultilevelCacheClient), serviceProvider
            => serviceProvider.GetRequiredService<IManualMultilevelCacheClient>());

        services.TryAddSingleton<ITypeAliasFactory, DefaultTypeAliasFactory>();
        services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(name));

        TryAddDistributedCacheCore(services, name);
        return services;
    }
}