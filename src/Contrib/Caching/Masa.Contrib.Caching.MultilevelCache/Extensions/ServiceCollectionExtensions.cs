// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheOptions> distributedCacheAction,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null,
        Action<MultilevelCacheOptions>? multilevelCacheOptionsAction = null)
    {
        if (multilevelCacheOptionsAction == null)
        {
            return services.AddMultilevelCache(
                distributedCacheAction,
                Const.DEFAULT_SECTION_NAME,
                typeAliasOptionsAction);
        }
        return services.AddMultilevelCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            distributedCacheAction,
            typeAliasOptionsAction,
            multilevelCacheOptionsAction);
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheOptions> distributedCacheAction,
        string sectionName,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
        => services.AddMultilevelCache(
            distributedCacheAction,
            sectionName,
            false,
            typeAliasOptionsAction);

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheOptions> distributedCacheAction,
        string sectionName,
        bool isReset = false,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
        => services.AddMultilevelCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            distributedCacheAction,
            sectionName,
            isReset,
            typeAliasOptionsAction);

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheOptions> distributedCacheAction,
        string sectionName = Const.DEFAULT_SECTION_NAME,
        bool isReset = false,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        services.AddMultilevelCache(name, sectionName, isReset, typeAliasOptionsAction);
        var distributedCacheOptions = new DistributedCacheOptions(services, name);
        distributedCacheAction.Invoke(distributedCacheOptions);
        return services;
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheOptions> distributedCacheAction,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null,
        Action<MultilevelCacheOptions>? multilevelCacheOptionsAction = null)
    {
        MultilevelCacheOptions multilevelCacheOptions = new();
        multilevelCacheOptionsAction?.Invoke(multilevelCacheOptions);
        services.AddMultilevelCache(name, multilevelCacheOptions, typeAliasOptionsAction);
        var distributedCacheOptions = new DistributedCacheOptions(services, name);
        distributedCacheAction.Invoke(distributedCacheOptions);

        return services;
    }

    #region internal methods

    internal static void AddMultilevelCache(
        this IServiceCollection services,
        string name,
        MultilevelCacheOptions multilevelCacheOptions,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddMultilevelCacheCore(services, name);

        services.Configure<MultilevelCacheFactoryOptions>(options =>
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
                    multilevelCacheOptions.SubscribeKeyPrefix,
                    serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(name)
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        if (typeAliasOptionsAction != null)
            services.Configure(name, typeAliasOptionsAction);
    }

    internal static void AddMultilevelCache(
        this IServiceCollection services,
        string name, string sectionName,
        bool isReset = false,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        Masa.BuildingBlocks.Caching.Extensions.ServiceCollectionExtensions.TryAddMultilevelCacheCore(services, name);

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
                    distributedCacheClientFactory.Create(name),
                    serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(name)
                );
                return multilevelCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        if (typeAliasOptionsAction != null)
            services.Configure(name, typeAliasOptionsAction);
    }

    #endregion

}
