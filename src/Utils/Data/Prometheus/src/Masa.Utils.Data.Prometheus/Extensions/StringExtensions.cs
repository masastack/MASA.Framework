// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

internal static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return default!;

        var span = new ReadOnlySpan<char>(str.ToArray());
        var c = span[0];
        if (c - 'A' >= 0 && c - 'Z' <= 0)
            return $"{(char)(c + 32)}{span[1..]}";

        return str;
    }
}
