// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public class MachineClockIdGenerator : BaseIdGenerator, ISnowflakeGenerator
{
    public MachineClockIdGenerator(IWorkerProvider workerProvider, IdGeneratorOptions idGeneratorOptions)
        : base(workerProvider, idGeneratorOptions)
    {
        LastTimestamp = GetCurrentTimestamp();
    }

    public override long Create()
    {
        lock (Lock)
        {
            var timestamp = LastTimestamp;
            Sequence = (Sequence + 1) & SequenceMask;
            if (Sequence == 0) timestamp = TilNextMillis(LastTimestamp);

            LastTimestamp = timestamp;

            return NextId(timestamp - Twepoch);
        }
    }

    protected override long TilNextMillis(long lastTimestamp) => lastTimestamp + 1;
}
