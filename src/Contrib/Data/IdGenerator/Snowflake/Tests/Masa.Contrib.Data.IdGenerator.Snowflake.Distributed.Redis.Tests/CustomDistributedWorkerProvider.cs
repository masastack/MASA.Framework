// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests;

public class CustomDistributedWorkerProvider : DistributedWorkerProvider
{
    public CustomDistributedWorkerProvider(IDistributedCacheClient distributedCacheClient,
        DistributedIdGeneratorOptions? distributedIdGeneratorOptions,
        RedisConfigurationOptions redisOptions,
        ILogger<DistributedWorkerProvider>? logger)
        : base(distributedCacheClient, distributedIdGeneratorOptions, redisOptions, logger)
    {
    }

    protected override Task<long?> GetWorkerIdByLogOutAsync()
    {
        return Task.FromResult<long?>(null);
    }
}
