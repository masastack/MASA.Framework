// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class MachineClockIdGenerator : Snowflake.MachineClockIdGenerator
{
    private readonly string _lastTimestampKey = "snowflake.last_timestamp";
    private long _lastRefreshTimestamp;
    private readonly long _refreshTimestampInterval;
    private readonly BaseRedis _redis;

    public MachineClockIdGenerator(
        IDistributedCacheClient distributedCacheClient,
        IWorkerProvider workerProvider,
        RedisConfigurationOptions redisOptions,
        DistributedIdGeneratorOptions distributedIdGeneratorOptions)
        : base(workerProvider, distributedIdGeneratorOptions.IdGeneratorOptions)
    {
        _redis = new BaseRedis(distributedCacheClient, redisOptions);
        _refreshTimestampInterval = distributedIdGeneratorOptions.IdGeneratorOptions.TimestampType == TimestampType.Seconds ?
            long.Parse(Math.Ceiling(distributedIdGeneratorOptions.RefreshTimestampInterval / 1000m)
                .ToString(CultureInfo.InvariantCulture)) : distributedIdGeneratorOptions.RefreshTimestampInterval;
        if (_redis.Database.HashExists(_lastTimestampKey, GetHashField()))
        {
            LastTimestamp = long.Parse(_redis.Database.HashGet(_lastTimestampKey, GetHashField())) + _refreshTimestampInterval;
        }
    }

    public string GetHashField() => base.GetWorkerId().ToString();

    protected override long TilNextMillis(long lastTimestamp)
    {
        lastTimestamp += 1;
        if (lastTimestamp - _lastRefreshTimestamp >= _refreshTimestampInterval)
        {
            _redis.Database.HashSetAsync(_lastTimestampKey, GetHashField(), lastTimestamp);
            _lastRefreshTimestamp = lastTimestamp;
        }
        return lastTimestamp;
    }
}
