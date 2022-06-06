// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedSnowflake(this IServiceCollection services)
        => services.AddDistributedSnowflake(null);

    public static IServiceCollection AddDistributedSnowflake(this IServiceCollection services,
        Action<DistributedIdGeneratorOptions>? options)
    {
        DistributedIdGeneratorOptions? distributedIdGenerators = null;

        if (!services.Any(service => service.ServiceType == typeof(IDistributedCacheClientFactory) &&
                service.ImplementationType == typeof(RedisCacheClientFactory)))
            throw new MasaException("Please add first using AddMasaRedisCache");

        services.TryAddSingleton<IWorkerProvider>(serviceProvider
            => new DistributedWorkerProvider(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                distributedIdGenerators,
                serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        return services.AddSnowflake(idGeneratorOptions =>
        {
            var distributedIdGeneratorOption = new DistributedIdGeneratorOptions();
            options?.Invoke(distributedIdGeneratorOption);

            DistributedIdGeneratorOptions.CopyTo(distributedIdGeneratorOption, idGeneratorOptions);

            if (distributedIdGeneratorOption.EnableMachineClock)
            {
                services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                    => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IDistributedCacheClient>(),
                        serviceProvider.GetRequiredService<IWorkerProvider>(),
                        serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                        distributedIdGeneratorOption));
            }

            long defaultHeartbeatInterval = distributedIdGeneratorOption.HeartbeatInterval;
            if (distributedIdGeneratorOption.IdleTimeOut <= defaultHeartbeatInterval)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(distributedIdGenerators.IdleTimeOut)} must be greater than {defaultHeartbeatInterval}");
            }

            distributedIdGenerators = distributedIdGeneratorOption;
        });
    }
}
