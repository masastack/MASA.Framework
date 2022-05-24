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

        services.TryAddSingleton<IWorkerProvider>(serviceProvider => new DistributedWorkerProvider(distributedIdGenerators,
            serviceProvider.GetRequiredService<IOptionsMonitor<RedisConfigurationOptions>>()));

        return services.AddSnowflake(idGeneratorOptions =>
        {
            var distributedIdGeneratorOption = new DistributedIdGeneratorOptions(idGeneratorOptions);
            options?.Invoke(distributedIdGeneratorOption);

            long defaultHeartbeatinterval = 30 * 1000;
            if (distributedIdGeneratorOption.EnableRecycle && distributedIdGeneratorOption.RecycleTime <= defaultHeartbeatinterval)
            {
                throw new ArgumentOutOfRangeException($"{nameof(distributedIdGenerators.RecycleTime)} RecycleTime must be greater than {defaultHeartbeatinterval}");
            }

            distributedIdGenerators = distributedIdGeneratorOption;
        });
    }
}
