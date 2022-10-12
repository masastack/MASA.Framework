// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheOptions> distributedCacheAction,
        string sectionName = Const.DEFAULT_SECTION_NAME,
        bool isReset = false)
        => services.AddMultilevelCache(Microsoft.Extensions.Options.Options.DefaultName, distributedCacheAction, sectionName, isReset);

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheOptions> distributedCacheAction,
        string sectionName = Const.DEFAULT_SECTION_NAME,
        bool isReset = false)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddMultilevelCacheCore(services);

        services.AddConfigure<MultilevelCacheOptions>(sectionName, name);

        services.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IMultilevelCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClientFactory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
                var multilevelCacheClient = new MultilevelCacheClient(
                    name,
                    isReset,
                    serviceProvider.GetRequiredService<IOptionsMonitor<MultilevelCacheOptions>>(),
                    distributedCacheClientFactory.Create(name)
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        var distributedCacheOptions = new DistributedCacheOptions(services, name);
        distributedCacheAction.Invoke(distributedCacheOptions);
        return services;
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheOptions> distributedCacheAction,
        Action<MultilevelCacheOptions>? action)
        => services.AddMultilevelCache(Microsoft.Extensions.Options.Options.DefaultName, distributedCacheAction, action);

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheOptions> distributedCacheAction,
        Action<MultilevelCacheOptions>? action)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddMultilevelCacheCore(services);
        MultilevelCacheOptions multilevelCacheOptions = new();
        action?.Invoke(multilevelCacheOptions);
        services.AddMultilevelCache(name, multilevelCacheOptions);
        var distributedCacheOptions = new DistributedCacheOptions(services, name);
        distributedCacheAction.Invoke(distributedCacheOptions);
        return services;
    }

    #region private methods

    internal static void AddMultilevelCache(this IServiceCollection service, string name, MultilevelCacheOptions multilevelCacheOptions)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddMultilevelCacheCore(service);

        service.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IMultilevelCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClientFactory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
                var multilevelCacheClient = new MultilevelCacheClient(
                    new MemoryCache(multilevelCacheOptions),
                    distributedCacheClientFactory.Create(name),
                    multilevelCacheOptions.CacheEntryOptions,
                    multilevelCacheOptions.SubscribeKeyType,
                    multilevelCacheOptions.GlobalCacheOptions,
                    multilevelCacheOptions.SubscribeKeyPrefix
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });
    }

    #endregion

}
