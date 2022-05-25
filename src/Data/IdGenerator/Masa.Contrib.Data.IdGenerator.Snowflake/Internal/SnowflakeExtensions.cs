// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

internal static class SnowflakeExtensions
{
    public static long GetTimestamp(this DateTimeOffset dateTimeOffset, uint timeType)
    {
        if (timeType == 1)
            return dateTimeOffset.ToUnixTimeMilliseconds();

        return dateTimeOffset.ToUnixTimeSeconds();
    }
}
