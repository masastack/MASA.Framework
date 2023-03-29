// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public static class DistributedCacheBuilderExtensions
{
    public static void UseCustomDistributedCache(
        this DistributedCacheBuilder distributedCacheBuilder,
        Func<IServiceProvider, IManualDistributedCacheClient> func)
    {
        distributedCacheBuilder.Services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == distributedCacheBuilder.Name))
                return;

            options.Options.Add(new MasaRelationOptions<IManualDistributedCacheClient>(distributedCacheBuilder.Name, func));
        });

        distributedCacheBuilder.Services.TryAddDistributedCache(distributedCacheBuilder.Name);
    }
}
