// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheBuilder> distributedCacheAction,
        Action<MultilevelCacheGlobalOptions>? multilevelCacheOptionsAction = null,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        if (multilevelCacheOptionsAction == null)
        {
            return services.AddMultilevelCache(
                distributedCacheAction,
                MultilevelCacheConstant.DEFAULT_SECTION_NAME,
                typeAliasOptionsAction);
        }
        return services.AddMultilevelCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            distributedCacheAction,
            multilevelCacheOptionsAction,
            typeAliasOptionsAction);
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        Action<DistributedCacheBuilder> distributedCacheAction,
        string sectionName,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
        => services.AddMultilevelCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            distributedCacheAction,
            sectionName,
            typeAliasOptionsAction);

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheBuilder> distributedCacheAction,
        string sectionName = MultilevelCacheConstant.DEFAULT_SECTION_NAME,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        services.AddMultilevelCache(name, sectionName, typeAliasOptionsAction);
        var distributedCacheBuilder = new DistributedCacheBuilder(services, name);
        distributedCacheAction.Invoke(distributedCacheBuilder);
        return services;
    }

    private static void AddMultilevelCache(
        this IServiceCollection services,
        string name,
        string sectionName,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        services.AddConfigure<MultilevelCacheGlobalOptions>(sectionName, name);

        var multilevelCacheBuilder = new MultilevelCacheBuilder(services, name);
        multilevelCacheBuilder.UseCustomMultilevelCache(serviceProvider =>
        {
            var multilevelCacheGlobalOptions = ComponentConfigUtils.GetComponentConfigByExecute(
                serviceProvider,
                name,
                sectionName,
                () =>
                {
                    var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<MultilevelCacheGlobalOptions>>();
                    return optionsMonitor.Get(name);
                });

            var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(name);
            var multilevelCacheClient = new MultilevelCacheClient(
                new MemoryCache(multilevelCacheGlobalOptions),
                distributedCacheClient,
                new MultilevelCacheOptions()
                {
                    CacheKeyType = multilevelCacheGlobalOptions.GlobalCacheOptions.CacheKeyType,
                    MemoryCacheEntryOptions = multilevelCacheGlobalOptions.CacheEntryOptions
                },
                multilevelCacheGlobalOptions.SubscribeKeyType,
                multilevelCacheGlobalOptions.SubscribeKeyPrefix,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(name),
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                multilevelCacheGlobalOptions.InstanceId
            );
            return multilevelCacheClient;
        });

        services.AddTypeAlias(name, typeAliasOptionsAction);
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Func<IServiceProvider, IManualMultilevelCacheClient> func)
    {
        var multilevelCacheBuilder = new MultilevelCacheBuilder(services, name);
        multilevelCacheBuilder.UseCustomMultilevelCache(func);
        return services;
    }

    public static IServiceCollection AddMultilevelCache(
        this IServiceCollection services,
        string name,
        Action<DistributedCacheBuilder> distributedCacheAction,
        Action<MultilevelCacheGlobalOptions>? multilevelCacheOptionsAction,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        MasaArgumentException.ThrowIfNull(distributedCacheAction);

        var distributedCacheOptions = new DistributedCacheBuilder(services, name);
        distributedCacheAction.Invoke(distributedCacheOptions);

        var multilevelCacheBuilder = new MultilevelCacheBuilder(services, name);
        multilevelCacheBuilder.UseCustomMultilevelCache(serviceProvider =>
        {
            MultilevelCacheGlobalOptions multilevelCacheGlobalOptions = new();
            multilevelCacheOptionsAction?.Invoke(multilevelCacheGlobalOptions);

            var distributedCacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(name);
            var multilevelCacheClient = new MultilevelCacheClient(
                new MemoryCache(multilevelCacheGlobalOptions),
                distributedCacheClient,
                new MultilevelCacheOptions
                {
                    CacheKeyType = multilevelCacheGlobalOptions.GlobalCacheOptions.CacheKeyType,
                    MemoryCacheEntryOptions = multilevelCacheGlobalOptions.CacheEntryOptions
                },
                multilevelCacheGlobalOptions.SubscribeKeyType,
                multilevelCacheGlobalOptions.SubscribeKeyPrefix,
                serviceProvider.GetRequiredService<ITypeAliasFactory>().Create(name),
                serviceProvider.GetService<IFormatCacheKeyProvider>(),
                multilevelCacheGlobalOptions.InstanceId
            );
            return multilevelCacheClient;

        });
        return services.AddTypeAlias(name, typeAliasOptionsAction);
    }

    private static IServiceCollection AddTypeAlias(
        this IServiceCollection services,
        string name,
        Action<TypeAliasOptions>? typeAliasOptionsAction = null)
    {
        if (typeAliasOptionsAction != null)
            services.Configure(name, typeAliasOptionsAction);
        return services;
    }
}
