// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public static class IdGeneratorOptionsExtensions
{
    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        Action<DistributedIdGeneratorOptions>? action = null)
        => snowflakeGeneratorOptions.UseRedis(Options.DefaultName, action);

    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        string name,
        Action<DistributedIdGeneratorOptions>? action = null)
    {
        var serviceProvider = snowflakeGeneratorOptions.Services.BuildServiceProvider();
        var redisConfigurationOptions = serviceProvider.GetService<IOptions<RedisConfigurationOptions>>();
        if (redisConfigurationOptions == null)
            throw new MasaException("Please add first using services.AddDistributedCache(options => options.UseStackExchangeRedisCache())");

        snowflakeGeneratorOptions.UseRedis(name, action, redisConfigurationOptions.Value);
    }

    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        Action<DistributedIdGeneratorOptions>? action,
        RedisConfigurationOptions redisConfigurationOptions)
        => snowflakeGeneratorOptions.UseRedis(Options.DefaultName, action, redisConfigurationOptions);

    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        string name,
        Action<DistributedIdGeneratorOptions>? action,
        RedisConfigurationOptions redisConfigurationOptions)
    {
        snowflakeGeneratorOptions.EnableSupportDistributed();
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(snowflakeGeneratorOptions);
        action?.Invoke(distributedIdGeneratorOptions);

        if (snowflakeGeneratorOptions.Services.All(service => service.ServiceType != typeof(IDistributedCacheClientFactory)))
            throw new MasaException("Please add first using services.AddDistributedCache(options => options.UseStackExchangeRedisCache())");

        if (distributedIdGeneratorOptions.IdleTimeOut <= snowflakeGeneratorOptions.HeartbeatInterval)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(distributedIdGeneratorOptions.IdleTimeOut)} must be greater than {snowflakeGeneratorOptions.HeartbeatInterval}");
        }

        snowflakeGeneratorOptions.Services.TryAddSingleton<IWorkerProvider>(serviceProvider
            => new DistributedWorkerProvider(serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(name),
                distributedIdGeneratorOptions,
                redisConfigurationOptions,
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        if (snowflakeGeneratorOptions.EnableMachineClock)
        {
            snowflakeGeneratorOptions.Services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.MachineClockIdGenerator(
                    serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().Create(name),
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    redisConfigurationOptions,
                    distributedIdGeneratorOptions));
        }
    }
}
