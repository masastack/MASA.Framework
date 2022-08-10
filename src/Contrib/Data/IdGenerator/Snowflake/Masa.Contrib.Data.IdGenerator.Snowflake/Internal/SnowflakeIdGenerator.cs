// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

public class SnowflakeIdGenerator : BaseIdGenerator, ISnowflakeGenerator
{
    public SnowflakeIdGenerator(IWorkerProvider workerProvider, SnowflakeGeneratorOptions snowflakeGeneratorOptions)
        : base(workerProvider, snowflakeGeneratorOptions)
    {
    }

    protected override (bool Support, long LastTimestamp) TimeCallBack(long currentTimestamp)
    {
        if ((TimestampType == TimestampType.Milliseconds && LastTimestamp - currentTimestamp <= MaxCallBackTime) ||
            (TimestampType == TimestampType.Seconds && LastTimestamp - currentTimestamp <= Math.Floor(MaxCallBackTime / 1000m)))
            return (true, TilNextMillis(LastTimestamp));

        return (false, 0);
    }
}
