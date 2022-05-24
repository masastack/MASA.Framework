// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis;

public class DistributedIdGeneratorOptions : IdGeneratorOptions
{
    /// <summary>
    /// WorkId allows recycling
    /// After the circular use is enabled, when the WorkerId is allocated,
    /// the WorkId that has not been refreshed after the timeout will be checked and reused.
    /// When a service cannot be connected to redis for a long time, the workId may be used by other applications.
    /// </summary>
    public bool EnableRecycle { get; set; }

    /// <summary>
    /// unit: ms
    /// </summary>
    /// <returns></returns>
    public long RecycleTime { get; set; } = 300 * 1000;

    public DistributedIdGeneratorOptions()
    {
    }

    internal DistributedIdGeneratorOptions(IdGeneratorOptions idGeneratorOptions) : this()
    {
        base.BaseTime = idGeneratorOptions.BaseTime;
        base.SequenceBits = idGeneratorOptions.SequenceBits;
        base.WorkerIdBits = idGeneratorOptions.WorkerIdBits;
        base.EnableMachineClock = idGeneratorOptions.EnableMachineClock;
    }
}
