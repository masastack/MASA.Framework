// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.Internal;

internal static class SnowflakeExtensions
{
    /// <summary>
    /// get timestamp
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="timestampType">Timestamp type: milliseconds: 1, seconds: 2</param>
    /// <returns></returns>
    public static long GetTimestamp(this DateTimeOffset dateTimeOffset, TimestampType timestampType)
    {
        if (timestampType == TimestampType.Milliseconds)
            return dateTimeOffset.ToUnixTimeMilliseconds();

        return dateTimeOffset.ToUnixTimeSeconds();
    }
}
