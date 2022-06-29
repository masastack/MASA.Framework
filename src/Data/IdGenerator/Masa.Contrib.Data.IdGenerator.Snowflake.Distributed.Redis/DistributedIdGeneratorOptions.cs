// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedIdGeneratorOptions : IdGeneratorOptions
{
    /// <summary>
    /// When there is no available WorkerId, Recycle idle and unused WorkerIds and recycle them
    /// default: 120000ms(2min)
    /// </summary>
    /// <returns></returns>
    public long IdleTimeOut { get; set; } = 2 * 60 * 1000;

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

    internal void CopyTo(IdGeneratorOptions idGeneratorOptions)
    {
        idGeneratorOptions.BaseTime = BaseTime;
        idGeneratorOptions.SequenceBits = SequenceBits;
        idGeneratorOptions.WorkerIdBits = WorkerIdBits;
        idGeneratorOptions.EnableMachineClock = EnableMachineClock;
        idGeneratorOptions.HeartbeatInterval = HeartbeatInterval;
        idGeneratorOptions.MaxExpirationTime = MaxExpirationTime;
        idGeneratorOptions.TimestampType = TimestampType;
        idGeneratorOptions.EnableSupportDistributed();
    }
}
