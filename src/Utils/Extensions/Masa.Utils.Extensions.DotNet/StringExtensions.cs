// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
        => string.IsNullOrEmpty(value);

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

    public static byte[] ConvertToBytes(this string value, Encoding encoding)
        => encoding.GetBytes(value);

    public static byte[] FromBase64String(this string value)
        => Convert.FromBase64String(value);

    public static string GetSpecifiedLengthString(
        this string value,
        int length,
        Action action,
        FillType fillType = FillType.NoFile,
        char fillCharacter = ' ')
    {
        if (fillType == FillType.NoFile && value.Length < length)
            action.Invoke();

        var keyLength = value.Length;
        if (keyLength == length) return value;

        if (keyLength > length) return value.Substring(0, length);

        if (fillType == FillType.Left) return value.PadLeft(length, fillCharacter);

        if (fillType == FillType.Right) return value.PadRight(length, fillCharacter);

        throw new NotSupportedException($"... Unsupported {nameof(fillType)}");
    }

    /// <summary>
    /// Support conversion from CamelCase, LowerCamelCase to SnakeCase
    /// Example: UserName -> User_Name, userName -> User_Name
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public static string CamelCaseToSnakeCase(this string value, CultureInfo? cultureInfo = null)
    {
        var stringBuilder = new StringBuilder();
        var index = 0;
        var delimiter = '_';
        foreach (var c in value.ToCharArray())
        {
            index++;
            if (index == 1)
            {
                if (Char.IsUpper(c)) stringBuilder.Append(c);
                else if (cultureInfo != null) stringBuilder.Append(Char.ToUpper(c, cultureInfo));
                else stringBuilder.Append(Char.ToUpper(c));
                continue;
            }

            if (Char.IsUpper(c)) stringBuilder.Append($"{delimiter}{c}");
            else stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Support conversion from CamelCase, LowerCamelCase to SnakeCase
    /// Example: UserName -> user_name, userName -> user_name
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public static string CamelCaseToLowerSnakeCase(this string value, CultureInfo? cultureInfo = null)
    {
        var stringBuilder = new StringBuilder();
        var index = 0;
        var delimiter = '_';
        foreach (var c in value.ToCharArray())
        {
            index++;
            if (index == 1)
            {
                if (Char.IsLower(c)) stringBuilder.Append(c);
                else if (cultureInfo != null) stringBuilder.Append(Char.ToLower(c, cultureInfo));
                else stringBuilder.Append(Char.ToLower(c));
                continue;
            }

            if (Char.IsUpper(c))
            {
                if (cultureInfo != null) stringBuilder.Append($"{delimiter}{Char.ToLower(c, cultureInfo)}");
                else stringBuilder.Append($"{delimiter}{Char.ToLower(c)}");
            }
            else stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Support conversion from SnakeCase, LowerSnakeCase to CamelCase
    /// Example: User_Name -> UserName, user_name -> UserName, user_Name -> UserName, User_name -> UserName
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public static string SnakeCaseToCamelCase(this string value, CultureInfo? cultureInfo = null)
        => value.ConvertToCamelCaseCore('_', cultureInfo);

    /// <summary>
    /// Support conversion from SnakeCase, LowerSnakeCase to CamelCase
    /// Example: User_Name -> userName, user_name -> userName, user_Name -> userName, User_name -> userName
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    public static string SnakeCaseToLowerCamelCase(this string value, CultureInfo? cultureInfo = null)
        => value.ConvertToLowerCamelCaseCore('_', cultureInfo);

    public static string ConvertToCamelCase(this string value, char delimiter, CultureInfo? cultureInfo = null)
    {
        if (value.IsNullOrEmpty()) return value;

        if (value.Contains(delimiter)) return value.ConvertToCamelCaseCore(delimiter, cultureInfo);

        var stringBuilder = new StringBuilder();
        var c = value[0];
        if (Char.IsUpper(c)) return value;

        stringBuilder.Append(cultureInfo != null ? Char.ToUpper(c, cultureInfo) : Char.ToUpper(c));
        if (value.Length > 1) stringBuilder.Append(value.Substring(1));
        return stringBuilder.ToString();
    }

    public static string ConvertToLowerCamelCase(this string value, char delimiter, CultureInfo? cultureInfo = null)
    {
        if (value.IsNullOrEmpty()) return value;

        if (value.Contains(delimiter)) return value.ConvertToLowerCamelCaseCore(delimiter, cultureInfo);

        var c = value[0];
        if (Char.IsLower(c)) return value;

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(cultureInfo != null ? Char.ToLower(c, cultureInfo) : Char.ToLower(c));
        if (value.Length > 1) stringBuilder.Append(value.Substring(1));
        return stringBuilder.ToString();
    }

    private static string ConvertToCamelCaseCore(this string value, char delimiter, CultureInfo? cultureInfo = null)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in value.Split(delimiter))
        {
            if (item.Length == 0)
                continue;

            char? c = item.ToArray()[0];
            if (Char.IsUpper(c.Value)) stringBuilder.Append(c);
            else if (cultureInfo != null) stringBuilder.Append(Char.ToUpper(c.Value, cultureInfo));
            else stringBuilder.Append(Char.ToUpper(c.Value));

            if (item.Length > 1) stringBuilder.Append(item.Substring(1));
        }
        return stringBuilder.ToString();
    }

    private static string ConvertToLowerCamelCaseCore(this string value, char delimiter, CultureInfo? cultureInfo = null)
    {
        var stringBuilder = new StringBuilder();
        var index = 0;
        foreach (var item in value.Split(delimiter))
        {
            if (item.Length == 0)
                continue;

            index++;

            char? c = item.ToArray()[0];

            if (index == 1)
            {
                if (Char.IsLower(c.Value)) stringBuilder.Append(c);
                else if (cultureInfo != null) stringBuilder.Append(Char.ToLower(c.Value, cultureInfo));
                else stringBuilder.Append(Char.ToLower(c.Value));
            }
            else
            {
                if (Char.IsUpper(c.Value)) stringBuilder.Append(c);
                else if (cultureInfo != null) stringBuilder.Append(Char.ToUpper(c.Value, cultureInfo));
                else stringBuilder.Append(Char.ToUpper(c.Value));
            }

            if (item.Length > 1) stringBuilder.Append(item.Substring(1));
        }
        return stringBuilder.ToString();
    }
}
