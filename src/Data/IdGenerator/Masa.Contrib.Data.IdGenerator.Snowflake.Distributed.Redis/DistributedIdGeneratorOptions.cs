// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedIdGeneratorOptions : IdGeneratorOptions
{
    /// <summary>
    /// When there is no available WorkerId, recover the inactive WorkerId for 2 minutes
    /// default: 120000ms(2min)
    /// unit: ms
    /// </summary>
    /// <returns></returns>
    public long RecycleTime { get; set; } = 2 * 60 * 1000;

    /// <summary>
    /// Get the minimum interval for WorkerId
    /// unit: ms
    /// </summary>
    public long GetWorkerIdMinInterval { get; set; } = 5 * 1000;

    public DistributedIdGeneratorOptions()
    {
        SupportDistributed = true;
    }

    internal DistributedIdGeneratorOptions(IdGeneratorOptions idGeneratorOptions) : this()
    {
        base.BaseTime = idGeneratorOptions.BaseTime;
        base.SequenceBits = idGeneratorOptions.SequenceBits;
        base.WorkerIdBits = idGeneratorOptions.WorkerIdBits;
        base.EnableMachineClock = idGeneratorOptions.EnableMachineClock;
    }
}
