// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc;

public static class ServiceCollectionExtensions
{
    public static void AddDccClient(this IServiceCollection services)
    {
        var options = AppSettings.GetModel<RedisConfigurationOptions>("DccOptions:RedisOptions");
        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, options);

        services.AddSingleton<IDccClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>()
            .CreateClient(DEFAULT_CLIENT_NAME);

            return new DccClient(client);
        });
    }

    public static void AddDccClient(this IServiceCollection services, Action<RedisConfigurationOptions> options)
    {
        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, options);

        services.AddSingleton<IDccClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>()
            .CreateClient(DEFAULT_CLIENT_NAME);

            return new DccClient(client);
        });
    }

    public static void AddDccClient(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, options);

        services.AddSingleton<IDccClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>()
            .CreateClient(DEFAULT_CLIENT_NAME);

            return new DccClient(client);
        });
    }
}
