// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias SnowflakeRedis;
global using SnowflakeRedis::Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.Tests;

public class CustomDistributedWorkerProvider : DistributedWorkerProvider
{
    public CustomDistributedWorkerProvider(
        DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        RedisConfigurationOptions redisOptions,
        ILoggerFactory? loggerFactory)
        : base(distributedIdGeneratorOptions, redisOptions, loggerFactory?.CreateLogger<DistributedWorkerProvider>())
    {
    }

    protected override Task<long?> GetWorkerIdByLogOutAsync()
    {
        return Task.FromResult<long?>(null);
    }

    public Task<long?> TestGetWorkerIdByLogOutAsync() => base.GetWorkerIdByLogOutAsync();
}
