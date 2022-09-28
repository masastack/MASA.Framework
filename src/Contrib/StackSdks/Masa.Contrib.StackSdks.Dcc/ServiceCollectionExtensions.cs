// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddDccClient(this IServiceCollection services)
    {
        var redisConfigurationOptions = AppSettings.GetModel<RedisConfigurationOptions>("DccOptions:RedisOptions");
        services.AddDccClient(redisConfigurationOptions);
    }

    public static void AddDccClient(this IServiceCollection services, Action<RedisConfigurationOptions> options)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        options.Invoke(redisConfigurationOptions);
        services.AddDccClient(redisConfigurationOptions);
    }

    public static void AddDccClient(this IServiceCollection services, RedisConfigurationOptions options)
    {
        services.AddStackExchangeRedisCache(DEFAULT_CLIENT_NAME, options);

        services.AddSingleton<IDccClient>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(DEFAULT_CLIENT_NAME);

            return new DccClient(client);
        });
    }
}
