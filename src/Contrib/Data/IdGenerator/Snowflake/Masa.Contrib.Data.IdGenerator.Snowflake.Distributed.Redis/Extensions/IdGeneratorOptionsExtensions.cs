// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public static class IdGeneratorOptionsExtensions
{
    public static void UseRedis(
        this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        Action<DistributedIdGeneratorOptions>? action = null)
    {
        snowflakeGeneratorOptions.UseRedis(action, options =>
        {
            options.Servers.Add(new ());
        });
    }

    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        Action<DistributedIdGeneratorOptions>? action,
        Action<RedisConfigurationOptions> redisConfigure)
    {
        var redisConfigurationOptions = new RedisConfigurationOptions();
        redisConfigure.Invoke(redisConfigurationOptions);
        snowflakeGeneratorOptions.UseRedis(action, redisConfigurationOptions);
    }

    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions,
        Action<DistributedIdGeneratorOptions>? action,
        RedisConfigurationOptions redisConfigurationOptions)
    {
        snowflakeGeneratorOptions.EnableSupportDistributed();
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(snowflakeGeneratorOptions);
        action?.Invoke(distributedIdGeneratorOptions);

        if (distributedIdGeneratorOptions.IdleTimeOut <= snowflakeGeneratorOptions.HeartbeatInterval)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(distributedIdGeneratorOptions.IdleTimeOut)} must be greater than {snowflakeGeneratorOptions.HeartbeatInterval}");
        }

        snowflakeGeneratorOptions.Services.TryAddSingleton<IWorkerProvider>(serviceProvider
            => new DistributedWorkerProvider(
                distributedIdGeneratorOptions,
                redisConfigurationOptions,
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        if (snowflakeGeneratorOptions.EnableMachineClock)
        {
            snowflakeGeneratorOptions.Services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.MachineClockIdGenerator(
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    redisConfigurationOptions,
                    distributedIdGeneratorOptions));
        }
    }
}
