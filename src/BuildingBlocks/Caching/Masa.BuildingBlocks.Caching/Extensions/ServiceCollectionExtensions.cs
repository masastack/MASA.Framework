// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        Action<DistributedCacheBuilder> action,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
        => services.AddDistributedCache(Microsoft.Extensions.Options.Options.DefaultName, action, typeAliasOptionsAction);

    public static IServiceCollection AddDistributedCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheBuilder> action,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        services.TryAddDistributedCache();
        DistributedCacheBuilder distributedCacheBuilder = new(services, name);
        action.Invoke(distributedCacheBuilder);

        if (typeAliasOptionsAction != null)
            services.Configure(name, typeAliasOptionsAction);

        return services;
    }
}
