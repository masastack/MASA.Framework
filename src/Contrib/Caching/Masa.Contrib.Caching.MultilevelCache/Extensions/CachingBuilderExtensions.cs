// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add multi-level cache
    /// </summary>
    /// <param name="cachingBuilder"></param>
    /// <param name="sectionName">MultilevelCache node name, not required, default: MultilevelCache(Use local configuration)</param>
    /// <param name="isReset">Whether to reset the MemoryCache after configuration changes</param>
    /// <returns></returns>
    public static ICachingBuilder AddMultilevelCache(
        this ICachingBuilder cachingBuilder,
        string sectionName = Const.DEFAULT_SECTION_NAME,
        bool isReset = false)
    {
        cachingBuilder.Services.TryAddMultilevelCacheCore();

        cachingBuilder.Services.AddConfigure<MultilevelCacheOptions>(sectionName, cachingBuilder.Name);

        cachingBuilder.Services.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == cachingBuilder.Name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IMultilevelCacheClient>(cachingBuilder.Name, serviceProvider =>
            {
                var distributedCacheClientFactory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
                var multilevelCacheClient = new MultilevelCacheClient(
                    cachingBuilder.Name,
                    isReset,
                    serviceProvider.GetRequiredService<IOptionsMonitor<MultilevelCacheOptions>>(),
                    distributedCacheClientFactory.Create(cachingBuilder.Name)
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return cachingBuilder;
    }

    public static ICachingBuilder AddMultilevelCache(this ICachingBuilder cachingBuilder, Action<MultilevelCacheOptions> action)
    {
        var multilevelCacheOptions = new MultilevelCacheOptions();
        action.Invoke(multilevelCacheOptions);
        return cachingBuilder.AddMultilevelCache(multilevelCacheOptions);
    }

    public static ICachingBuilder AddMultilevelCache(this ICachingBuilder cachingBuilder, MultilevelCacheOptions multilevelCacheOptions)
    {
        ArgumentNullException.ThrowIfNull(cachingBuilder);

        cachingBuilder.Services.TryAddMultilevelCacheCore();

        cachingBuilder.Services.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == cachingBuilder.Name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IMultilevelCacheClient>(cachingBuilder.Name, serviceProvider =>
            {
                var distributedCacheClientFactory = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>();
                var multilevelCacheClient = new MultilevelCacheClient(
                    new MemoryCache(multilevelCacheOptions),
                    distributedCacheClientFactory.Create(cachingBuilder.Name),
                    multilevelCacheOptions.CacheEntryOptions,
                    multilevelCacheOptions.SubscribeKeyType,
                    multilevelCacheOptions.SubscribeKeyPrefix
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return cachingBuilder;
    }
}
