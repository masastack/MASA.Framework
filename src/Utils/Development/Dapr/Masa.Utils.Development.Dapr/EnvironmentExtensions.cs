// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Development.Dapr;

public static class EnvironmentExtensions
{
    public static void TryAdd(string environment, Func<string?> func, out bool isExist)
    {
        var value = Environment.GetEnvironmentVariable(environment);
        isExist = value == null;
        if (isExist)
        {
            Environment.SetEnvironmentVariable(environment, func.Invoke());
        }
    }
}
