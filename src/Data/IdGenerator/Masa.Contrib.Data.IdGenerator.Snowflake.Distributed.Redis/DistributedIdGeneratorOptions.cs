// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedIdGeneratorOptions : IdGeneratorOptions
{
    /// <summary>
    /// When there is no available WorkerId, recover the inactive WorkerId for 2 minutes
    /// default: 120000ms(2min)
    /// </summary>
    /// <returns></returns>
    public long RecycleTime { get; set; } = 2 * 60 * 1000;

    /// <summary>
    /// Get the minimum interval for WorkerId
    /// default: 5000ms
    /// </summary>
    public long GetWorkerIdMinInterval { get; set; } = 5 * 1000;

    /// <summary>
    /// refresh timestamp period
    /// default: 500ms
    /// </summary>
    public long RefreshTimestampInterval { get; set; } = 500;

    internal static void CopyTo(DistributedIdGeneratorOptions distributedIdGeneratorOptions, IdGeneratorOptions idGeneratorOptions)
    {
        idGeneratorOptions.BaseTime = distributedIdGeneratorOptions.BaseTime;
        idGeneratorOptions.SequenceBits = distributedIdGeneratorOptions.SequenceBits;
        idGeneratorOptions.WorkerIdBits = distributedIdGeneratorOptions.WorkerIdBits;
        idGeneratorOptions.EnableMachineClock = distributedIdGeneratorOptions.EnableMachineClock;
        idGeneratorOptions.HeartbeatInterval = distributedIdGeneratorOptions.HeartbeatInterval;
        idGeneratorOptions.MaxExpirationTime = distributedIdGeneratorOptions.MaxExpirationTime;
        idGeneratorOptions.TimestampType = distributedIdGeneratorOptions.TimestampType;
        idGeneratorOptions.EnableSupportDistributed();
    }
}
