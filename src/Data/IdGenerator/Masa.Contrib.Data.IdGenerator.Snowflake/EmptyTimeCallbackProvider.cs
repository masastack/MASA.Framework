// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

/// <summary>
/// Time callback is not supported by default
/// </summary>
public class EmptyTimeCallbackProvider : ITimeCallbackProvider
{
    public long NextId(long currentTimestamp, long lastTimestamp, int timestampLeftShift, long workMachineId, int sequenceBits)
    {
        throw new Exception(
            $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {lastTimestamp - currentTimestamp} milliseconds");
    }
}
