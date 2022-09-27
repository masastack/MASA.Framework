// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

extern alias Snowflake;
using Snowflake.Masa.Contrib.Data.IdGenerator.Snowflake;

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Tests;

public class CustomSnowflakeIdGenerator : SnowflakeIdGenerator
{
    private long _nextMillis;

    public CustomSnowflakeIdGenerator(IWorkerProvider workerProvider, SnowflakeGeneratorOptions snowflakeGeneratorOptions)
        : base(workerProvider, snowflakeGeneratorOptions)
    {

    }

    public (bool Support, long LastTimestamp) TestTimeCallBack(long currentTimestamp)
        => base.TimeCallBack(currentTimestamp);

    public void SetLastTimestamp(long lastTimestamp)
    {
        LastTimestamp = lastTimestamp;
    }

    public void SetTilNextMillis(long nextMillis)
    {
        _nextMillis = nextMillis;
    }

    protected override long TilNextMillis(long lastTimestamp)
    {
        return _nextMillis;
    }
}
