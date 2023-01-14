// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDccClient(this IServiceCollection services, string sectionName = "DccOptions")
    {
        services.AddConfigure<RedisConfigurationOptions>($"{sectionName}:RedisOptions", DEFAULT_CLIENT_NAME);
        return services.AddDccClientCore();
    }

    public static IServiceCollection AddDccClient(this IServiceCollection services, Action<RedisConfigurationOptions> options)
    {
        services.Configure(DEFAULT_CLIENT_NAME, options);
        return services.AddDccClientCore();
    }

    public static IServiceCollection AddDccClient(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddDistributedCache(DEFAULT_CLIENT_NAME, distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache(options);
        });

        return services.AddDccClientCore(false);
    }

    private static IServiceCollection AddDccClientCore(this IServiceCollection services, bool isUseStackExchangeRedisCache = true)
    {
        if (isUseStackExchangeRedisCache)
            services.AddDistributedCache(DEFAULT_CLIENT_NAME, distributedCacheOptions =>
            {
                distributedCacheOptions.UseStackExchangeRedisCache();
            });

        services.AddSingleton<IDccClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);

            return new DccClient(client);
        });
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
