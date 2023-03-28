// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public static class MultilevelCacheBuilderExtensions
{
    public static void UseCustomMultilevelCache(
        this MultilevelCacheBuilder multilevelCacheBuilder,
        Func<IServiceProvider, IManualMultilevelCacheClient> func)
    {
        multilevelCacheBuilder.Services.Configure<MultilevelCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == multilevelCacheBuilder.Name))
                return;

            var cacheRelationOptions = new MasaRelationOptions<IManualMultilevelCacheClient>(multilevelCacheBuilder.Name, func.Invoke);
            options.Options.Add(cacheRelationOptions);
        });

        multilevelCacheBuilder.Services.TryAddMultilevelCache(multilevelCacheBuilder.Name);
    }
}
