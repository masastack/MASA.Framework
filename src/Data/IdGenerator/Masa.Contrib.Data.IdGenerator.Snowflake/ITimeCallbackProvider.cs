// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public interface ITimeCallbackProvider
{
    long NextId(long currentTimestamp, long lastTimestamp, int timestampLeftShift, long workMachineId, int sequenceBits);
}
