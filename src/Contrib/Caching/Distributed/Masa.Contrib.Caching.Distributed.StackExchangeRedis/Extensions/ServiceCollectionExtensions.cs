// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStackExchangeRedisCache(this IServiceCollection services,
        RedisConfigurationOptions redisConfigurationOptions,
        CacheEntryOptions? cacheEntryOptions = null)
        => services.AddStackExchangeRedisCache(
            Microsoft.Extensions.Options.Options.DefaultName,
            redisConfigurationOptions,
            cacheEntryOptions);

    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        string redisSectionName = "RedisConfig",
        CacheEntryOptions? cacheEntryOptions = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddStackExchangeRedisCacheCore();

        services.TryAddConfigure<RedisConfigurationOptions>(redisSectionName);

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, serviceProvider =>
            {
                IOptionsMonitor<RedisConfigurationOptions> redisConfigurationOptionsMonitor =
                    serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>();

                redisConfigurationOptionsMonitor.OnChange((option, optionName) =>
                {
                    if (optionName == name)
                    {
                        var func = options.Options.First(opt => opt.Name == name);

                        var client = func.Func.Invoke(serviceProvider);
                        if (client is DistributedCacheClient redisClient)
                            redisClient.RefreshRedisConfigurationOptions(option);
                    }
                });

                var distributedCacheClient = new DistributedCacheClient(
                    redisConfigurationOptionsMonitor.Get(name),
                    cacheEntryOptions,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return services;
    }

    public static IServiceCollection AddStackExchangeRedisCache(
        this IServiceCollection services,
        string name,
        RedisConfigurationOptions redisConfigurationOptions,
        CacheEntryOptions? cacheEntryOptions = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.AddStackExchangeRedisCacheCore();

        services.Configure<DistributedCacheFactoryOptions>(options =>
        {
            if (options.Options.Any(opt => opt.Name == name))
                return;

            var cacheRelationOptions = new CacheRelationOptions<IDistributedCacheClient>(name, serviceProvider =>
            {
                var distributedCacheClient = new DistributedCacheClient(
                    redisConfigurationOptions,
                    cacheEntryOptions,
                    jsonSerializerOptions
                );
                return distributedCacheClient;
            });
            options.Options.Add(cacheRelationOptions);
        });

        return services;
    }

    private static void AddStackExchangeRedisCacheCore(this IServiceCollection services)
    {
        if (services.Any(service => service.ImplementationType == typeof(RedisProvider)))
        {
            return;
        }
        services.TryAddConfigure<RedisConfigurationOptions>("");
        services.AddSingleton<RedisProvider>();

        // services.AddDefaultConfigurationByStackExchangeRedisCache(sectionName);
        services.AddCachingCore();
    }

    public static IServiceCollection TryAddConfigure<TOptions>(
        this IServiceCollection services,
        string sectionName)
        where TOptions : class
    {
        services.AddOptions();
        var serviceProvider = services.BuildServiceProvider();
        IConfiguration? configuration = serviceProvider.GetService<IMasaConfiguration>()?.Local ??
            serviceProvider.GetService<IConfiguration>();
        if (configuration == null)
            return services;

        string name = Microsoft.Extensions.Options.Options.DefaultName;
        var configurationSection = configuration.GetSection(sectionName);
        if (!configurationSection.Exists())
            return services;

        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, configuration));
        services.TryAddSingleton<IConfigureOptions<TOptions>>(new NamedConfigureFromConfigurationOptions<TOptions>(name,
            configuration, _ =>
            {
            }));
        return services;
    }

    // private static RedisConfigurationOptions GetRedisConfigurationOptions(this IServiceCollection services, string sectionName)
    // {
    //     var serviceProvider = services.BuildServiceProvider();
    //     IConfiguration configuration;
    //     if (services.Any(d => d.ServiceType == typeof(IMasaConfiguration)))
    //     {
    //         var redisConfigurationOptions = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>().Value;
    //         if (redisConfigurationOptions.Servers.Any())
    //             return redisConfigurationOptions;
    //
    //         configuration = serviceProvider.GetRequiredService<IMasaConfiguration>().Local;
    //     }
    //     else
    //     {
    //         configuration = serviceProvider.GetRequiredService<IConfiguration>();
    //     }
    //     var configurationSection = configuration.GetSection(sectionName);
    //     if (!configurationSection.Exists())
    //     {
    //         return new RedisConfigurationOptions()
    //         {
    //             Servers = new List<RedisServerOptions>()
    //             {
    //                 new()
    //             }
    //         };
    //     }
    //     return configurationSection.Get<RedisConfigurationOptions>() ?? new RedisConfigurationOptions()
    //     {
    //         Servers = new List<RedisServerOptions>()
    //         {
    //             new()
    //         }
    //     };
    // }

    private sealed class RedisProvider
    {

    }
}
