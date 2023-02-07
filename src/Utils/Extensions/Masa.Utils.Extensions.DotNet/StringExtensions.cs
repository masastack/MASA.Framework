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
        FillType fillType = FillType.NoFill,
        char fillCharacter = ' ')
    {
        if (fillType == FillType.NoFill && value.Length < length)
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
    public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
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
    public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (str.StartsWith(c.ToString(), comparisonType))
        {
            return str;
        }

        return c + str;
    }

    /// <summary>
    /// Converts new line const in the string to <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string NormalizeLineBreak(this string str)
    {
        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
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

    public static string ToFirstCharUpper(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return char.ToUpperInvariant(str[0]) + str.Substring(1);
    }

    public static string ToFirstCharLower(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// Conversion string to PascalCase.
    /// Example: User_Name -> UserName, user_name -> UserName, user_Name -> UserName, User_name -> UserName
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return string.Join("", str.Split(new[] { '-', '_', ' ' })
            .Where(s => !s.IsNullOrWhiteSpace())
            .Select(c => ToFirstCharUpper(c)));
    }

    /// <summary>
    /// Conversion string to CamelCase.
    /// Example: User_Name -> userName, user_name -> userName, user_Name -> userName, User_name -> userName
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }
        return string.Join("", str.Split(new[] { '-', '_', ' ' })
            .Where(s => !s.IsNullOrWhiteSpace())
            .Select((c, i) => i == 0 ? ToFirstCharLower(c) : ToFirstCharUpper(c)));
    }

    /// <summary>
    /// Conversion PascalCase/camelCase string to sentence (splitting words by space).
    /// Example: "ThisIsSampleSentence" is converted to "This is sample sentence".
    /// </summary>
    /// <param name="str"></param>
    public static string ToSentenceCase(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        return ToFirstCharUpper(ConvertCase(str, ' '));
    }

    /// <summary>
    /// Conversion PascalCase/camelCase string to kebab-case.
    /// Example: "UserName" is converted to "user-name".
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToKebabCase(this string str)
    {
        if (str.IsNullOrEmpty())
        {
            return str;
        }
        return ConvertCase(str, '-');
    }

    /// <summary>
    /// conversion PascalCase/camelCase string to snake case.
    /// Example: "UserName" is converted to "user_name".
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return str;
        }

        return ConvertCase(str, '_');
    }

#pragma warning disable S3776
    private static string ConvertCase(string str, char wordDelimiter)
    {
        var builder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
        var previousCategory = default(UnicodeCategory?);
        for (var currentIndex = 0; currentIndex < str.Length; currentIndex++)
        {
            var currentChar = str[currentIndex];
            if (currentChar == wordDelimiter)
            {
                builder.Append(wordDelimiter);
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
                        builder.Append(wordDelimiter);
                    }

                    currentChar = char.ToLower(currentChar);
                    break;

                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    if (previousCategory == UnicodeCategory.SpaceSeparator)
                    {
                        builder.Append(wordDelimiter);
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
#pragma warning restore S3776
}
