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
            => new DistributedWorkerProvider(distributedIdGenerators,
                serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                serviceProvider.GetService<ILogger<DistributedWorkerProvider>>()));

        return services.AddSnowflake(idGeneratorOptions =>
        {
            var distributedIdGeneratorOption = new DistributedIdGeneratorOptions();
            options?.Invoke(distributedIdGeneratorOption);

            DistributedIdGeneratorOptions.CopyTo(distributedIdGeneratorOption,idGeneratorOptions);

            if (distributedIdGeneratorOption.EnableMachineClock)
            {
                services.TryAddSingleton<IIdGenerator>(serviceProvider
                    => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IWorkerProvider>(),
                        serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>(),
                        distributedIdGeneratorOption));
            }

            long defaultHeartbeatinterval = 30 * 1000;
            if (distributedIdGeneratorOption.RecycleTime <= defaultHeartbeatinterval)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(distributedIdGenerators.RecycleTime)} RecycleTime must be greater than {defaultHeartbeatinterval}");
            }

            distributedIdGenerators = distributedIdGeneratorOption;
        });
    }
}
