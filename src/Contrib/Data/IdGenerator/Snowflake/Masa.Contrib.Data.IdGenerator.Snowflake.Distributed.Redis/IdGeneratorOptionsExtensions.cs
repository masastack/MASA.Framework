// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public static class IdGeneratorOptionsExtensions
{
    public static void UseRedis(this SnowflakeGeneratorOptions snowflakeGeneratorOptions, Action<DistributedIdGeneratorOptions>? action = null)
    {
        snowflakeGeneratorOptions.EnableSupportDistributed();
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(snowflakeGeneratorOptions);
        action?.Invoke(distributedIdGeneratorOptions);

        if (snowflakeGeneratorOptions.Services.All(service => service.ServiceType != typeof(IDistributedCacheClientFactory)))
            throw new MasaException("Please add first using AddStackExchangeRedisCache");

        if (distributedIdGeneratorOptions.IdleTimeOut <= snowflakeGeneratorOptions.HeartbeatInterval)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(distributedIdGeneratorOptions.IdleTimeOut)} must be greater than {snowflakeGeneratorOptions.HeartbeatInterval}");
        }

        snowflakeGeneratorOptions.Services.TryAddSingleton<IWorkerProvider>(serviceProvider
            => new DistributedWorkerProvider(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                distributedIdGeneratorOptions,
                serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        if (snowflakeGeneratorOptions.EnableMachineClock)
        {
            snowflakeGeneratorOptions.Services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                    distributedIdGeneratorOptions));
        }
    }
}
