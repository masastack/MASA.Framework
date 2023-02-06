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
    /// If the given string does not end with char, char is added to the end of the given string.
    /// </summary>
    public static string EnsureEndWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (str.EndsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return str + c;
    }

    /// <summary>
    /// If the given string does not start with char, char is added to the beginning of the given string.
    /// </summary>
    public static string EnsureStartWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (str.StartsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return c + str;
    }

    /// <summary>
    /// Gets a substring of a string from beginning of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
    public static string Left(this string str, int len)
    {
        if (str.Length < len)
        {
            throw new ArgumentException("len argument can`t be greater than string's length!");
        }

        return str.Substring(0, len);
    }

    /// <summary>
    /// Converts new line const in the string to <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string NormalizeLineBreak(this string str)
    {
        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
    }

    /// <summary>
    /// Deletes the first occurrence of the given suffix from the end of the given string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="suffixes">one or more suffix.</param>
    /// <returns>Modified string or the same string if it has not any of given suffixes</returns>
    public static string RemoveSuffix(this string str, params string[] suffixes)
    {
        return str.RemoveSuffix(StringComparison.Ordinal, suffixes);
    }

    /// <summary>
    /// Deletes the first occurrence of the given suffix from the end of the given string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="comparisonType">String comparison type</param>
    /// <param name="suffixes">one or more suffix.</param>
    /// <returns>Modified string or the same string if it has not any of given suffixes</returns>
    public static string RemoveSuffix(this string str, StringComparison comparisonType, params string[] suffixes)
    {
        if (str.IsNullOrEmpty() || suffixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var suffix in suffixes)
        {
            if (str.EndsWith(suffix, comparisonType))
            {
                return str.Left(str.Length - suffix.Length);
            }
        }

        return str;
    }

    /// <summary>
    /// Removes the first occurrence of the given prefix from the beginning of the given string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="preFixes">one or more prefix.</param>
    /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
    public static string RemovePreFix(this string str, params string[] preFixes)
    {
        return str.RemovePreFix(StringComparison.Ordinal, preFixes);
    }

    /// <summary>
    /// Removes the first occurrence of the given prefix from the beginning of the given string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="comparisonType">String comparison type</param>
    /// <param name="preFixes">one or more prefix.</param>
    /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
    public static string RemovePreFix(this string str, StringComparison comparisonType, params string[] preFixes)
    {
        if (str.IsNullOrEmpty() || preFixes.IsNullOrEmpty())
        {
            return str;
        }

        foreach (var preFix in preFixes)
        {
            if (str.StartsWith(preFix, comparisonType))
            {
                return str.Right(str.Length - preFix.Length);
            }
        }

        return str;
    }

    /// <summary>
    /// Gets a substring of a string from end of the string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
    public static string Right(this string str, int len)
    {
        if (str.Length < len)
        {
            throw new ArgumentException("len argument can`t be greater than string's length!");
        }

        return str.Substring(str.Length - len, len);
    }

    public static string[] Split(this string str, string separator)
    {
        return str.Split(new[] { separator }, StringSplitOptions.None);
    }

    public static string[] Split(this string str, string separator, StringSplitOptions options)
    {
        return str.Split(new[] { separator }, options);
    }

    /// <summary>
    /// split string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string str)
    {
        return str.Split(Environment.NewLine);
    }

    /// <summary>
    /// split string by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string[] SplitToLines(this string str, StringSplitOptions options)
    {
        return str.Split(Environment.NewLine, options);
    }

    /// <summary>
    /// Converts PascalCase string to camelCase string.
    /// </summary>
    /// <param name="str">String to convert</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    /// <param name="handleAbbreviations">set true to if you want to convert 'XYZ' to 'xyz'.</param>
    /// <returns>camelCase of the string</returns>
    public static string ToCamelCase(this string str, bool useCurrentCulture = false, bool handleAbbreviations = false)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        if (str.Length == 1)
        {
            return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
        }

        if (handleAbbreviations && IsAllUpperCase(str))
        {
            return useCurrentCulture ? str.ToLower() : str.ToLowerInvariant();
        }

        return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.Substring(1);
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
    /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
    /// </summary>
    /// <param name="str">String to convert.</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    public static string ToSentenceCase(this string str, bool useCurrentCulture = false)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        return useCurrentCulture
            ? Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]))
            : Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to kebab-case.
    /// </summary>
    /// <param name="str">String to convert.</param>
    /// <param name="useCurrentCulture">set true to use current culture. Otherwise, invariant culture will be used.</param>
    public static string ToKebabCase(this string str, bool useCurrentCulture = false)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }
        return Regex.Replace(str, @"(\B[A-Z])", "-$1").ToLower();
    }

    /// <summary>
    /// Converts given PascalCase/camelCase string to snake case.
    /// Example: "ThisIsSampleSentence" is converted to "this_is_a_sample_sentence".
    /// https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NameTranslation/NpgsqlSnakeCaseNameTranslator.cs#L51
    /// </summary>
    /// <param name="str">String to convert.</param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        var builder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
        var previousCategory = default(UnicodeCategory?);

        for (var currentIndex = 0; currentIndex < str.Length; currentIndex++)
        {
            var currentChar = str[currentIndex];
            if (currentChar == '_')
            {
                builder.Append('_');
                previousCategory = null;
                continue;
            }

            var currentCategory = char.GetUnicodeCategory(currentChar);
            switch (currentCategory)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                    if (previousCategory == UnicodeCategory.SpaceSeparator ||
                        previousCategory == UnicodeCategory.LowercaseLetter ||
                        previousCategory != UnicodeCategory.DecimalDigitNumber &&
                        previousCategory != null &&
                        currentIndex > 0 &&
                        currentIndex + 1 < str.Length &&
                        char.IsLower(str[currentIndex + 1]))
                    {
                        builder.Append('_');
                    }

                    currentChar = char.ToLower(currentChar);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append('_');
                    }
                    break;

                default:
                    if (previousCategory != null)
                    {
                        previousCategory = UnicodeCategory.SpaceSeparator;
                    }
                    continue;
            }

            builder.Append(currentChar);
            previousCategory = currentCategory;
        }

        return builder.ToString();
    }

    private static bool IsAllUpperCase(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
            {
                return false;
            }
        }

        return true;
    }
}
