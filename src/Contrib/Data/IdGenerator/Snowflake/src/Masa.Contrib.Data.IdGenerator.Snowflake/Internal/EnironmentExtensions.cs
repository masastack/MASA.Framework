// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

internal class EnironmentExtensions
{
    public static int? GetEnvironmentVariable(string variable)
    {
        var environmentVariable = Environment.GetEnvironmentVariable(variable);
        if (string.IsNullOrEmpty(environmentVariable))
            return null;

        return int.Parse(environmentVariable);
    }
}
