// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests.Infrastructure;

public class CustomerMachineClockIdGenerator : Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.MachineClockIdGenerator
{
    public CustomerMachineClockIdGenerator(IDistributedCacheClient distributedCacheClient,
        IWorkerProvider workerProvider,
        RedisConfigurationOptions redisOptions,
        DistributedIdGeneratorOptions distributedIdGeneratorOptions)
        : base(distributedCacheClient, workerProvider, redisOptions, distributedIdGeneratorOptions)
    {
    }

    public long TestTilNextMillis(long lastTimestamp) => base.TilNextMillis(lastTimestamp);
}
