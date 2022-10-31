// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);

    public static void CheckIsNullOrWhiteSpace(this string? value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (value.IsNullOrWhiteSpace())
            throw new ArgumentException($"{paramName} cannot be WhiteSpace or Null");
    }

    public static void CheckIsNullOrEmpty(this string? value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (value.IsNullOrEmpty())
            throw new ArgumentException($"{paramName} cannot be Empty or Null");
    }

    public static string TrimStart(this string value, string trimParameter)
        => value.TrimStart(trimParameter, StringComparison.CurrentCulture);

    public static string TrimStart(this string value,
        string trimParameter,
        StringComparison stringComparison)
    {
        if (!value.StartsWith(trimParameter, stringComparison))
            return value;

        return value.Substring(trimParameter.Length);
    }

    public static string TrimEnd(this string value, string trimParameter)
        => value.TrimEnd(trimParameter, StringComparison.CurrentCulture);

    public static string TrimEnd(this string value,
        string trimParameter,
        StringComparison stringComparison)
    {
        if (!value.EndsWith(trimParameter, stringComparison))
            return value;

        return value.Substring(0, value.Length - trimParameter.Length);
    }
}
