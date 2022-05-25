// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class MachineClockIdGenerator : Snowflake.MachineClockIdGenerator
{
    private readonly string _lastTimestampKey = "snowflake.last_timestamp";
    private long _lastRefreshTimestamp = 0;
    private readonly long _refreshTimestampInterval;
    private readonly BaseRedis _redis;

    public MachineClockIdGenerator(
        IWorkerProvider workerProvider,
        IOptions<RedisConfigurationOptions> redisOptions,
        DistributedIdGeneratorOptions distributedIdGeneratorOptions)
        : base(workerProvider, distributedIdGeneratorOptions)
    {
        _redis = new BaseRedis(redisOptions);
        _refreshTimestampInterval = distributedIdGeneratorOptions.TimestampType == 1 ?
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
