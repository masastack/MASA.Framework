// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.Distributed.StackExchangeRedis")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

internal static class DistributedCacheBuilderExtensions
{
    public static void AddDistributedCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        Func<IServiceProvider, IManualDistributedCacheClient> func)
    {
        distributedCacheBuilder.Services.Configure<TypeAliasFactoryOptions>(options => options.TryAdd(distributedCacheBuilder.Name));

        distributedCacheBuilder.Services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == distributedCacheBuilder.Name))
                return;

            options.Options.Add(new CacheRelationOptions<IManualDistributedCacheClient>(distributedCacheBuilder.Name, func));
        });
    }
}
