// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MedallionBuilderExtensions
{
    public static void UseRedis(this MedallionBuilder medallionBuilder,
        string connectionString,
        Action<RedisDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            =>
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase(), options);
        });
    }

    public static void UseRedis(this MedallionBuilder medallionBuilder,
        IDatabase database,
        Action<RedisDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new RedisDistributedSynchronizationProvider(database, options));
    }

    public static void UseRedis(this MedallionBuilder medallionBuilder,
        IEnumerable<IDatabase> databases,
        Action<RedisDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(_
            => new RedisDistributedSynchronizationProvider(databases, options));
    }

    public static void UseRedis(this MedallionBuilder medallionBuilder,
        Action<RedisDistributedSynchronizationOptionsBuilder>? options = null)
    {
        medallionBuilder.Services.AddSingleton<IDistributedLockProvider>(serviceProvider
            =>
        {
            var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisConfigurationOptions>>();
            var connection = ConnectionMultiplexer.Connect(ConfigurationOptionsExtensions.GetConfigurationOptions(redisOptions.Value));
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase(), options);
        });
    }
}
