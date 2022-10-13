// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, Action<DistributedCacheOptions> action)
        => services.AddDistributedCache(Microsoft.Extensions.Options.Options.DefaultName, action);

    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheOptions> action)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddDistributedCacheCore(services, name);
        DistributedCacheOptions options = new(services, name);
        action.Invoke(options);
        return services;
    }
}
