// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public static class IdGeneratorOptionsExtensions
{
    public static void UseRedis(this IdGeneratorOptions idGeneratorOptions, Action<DistributedIdGeneratorOptions>? action = null)
    {
        idGeneratorOptions.EnableSupportDistributed();
        DistributedIdGeneratorOptions distributedIdGeneratorOptions = new DistributedIdGeneratorOptions(idGeneratorOptions);
        action?.Invoke(distributedIdGeneratorOptions);

        if (!idGeneratorOptions.Services.Any(service => service.ServiceType == typeof(IDistributedCacheClientFactory) &&
                service.ImplementationType == typeof(RedisCacheClientFactory)))
            throw new MasaException("Please add first using AddMasaRedisCache");

        if (distributedIdGeneratorOptions.IdleTimeOut <= idGeneratorOptions.HeartbeatInterval)
        {
            throw new ArgumentOutOfRangeException(
                $"{nameof(distributedIdGeneratorOptions.IdleTimeOut)} must be greater than {idGeneratorOptions.HeartbeatInterval}");
        }

        idGeneratorOptions.Services.TryAddSingleton<IWorkerProvider>(serviceProvider
            => new DistributedWorkerProvider(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                distributedIdGeneratorOptions,
                serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        if (idGeneratorOptions.EnableMachineClock)
        {
            idGeneratorOptions.Services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                    distributedIdGeneratorOptions));
        }
    }

    private static TService GetInstance<TService>(this IServiceCollection services) where TService : notnull =>
        services.BuildServiceProvider().GetRequiredService<TService>();


}
