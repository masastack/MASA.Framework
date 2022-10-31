// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class EnvironmentUtils
{
    public static bool TrySetEnvironmentVariable(string environment, string? value)
    {
        if (Environment.GetEnvironmentVariable(environment).IsNullOrWhiteSpace())
        {
            Environment.SetEnvironmentVariable(environment, value);
            return true;
        }
        return false;
    }
}
