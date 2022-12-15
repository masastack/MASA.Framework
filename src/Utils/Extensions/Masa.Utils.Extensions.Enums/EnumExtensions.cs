// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class EnumExtensions
{
    public static string GetDescriptionValue(this Enum enumValue)
        => enumValue.GetCustomAttribute(() => new DescriptionAttribute(enumValue.ToString()))!.Description;

    public static DescriptionAttribute GetDescription(this Enum enumValue)
        => enumValue.GetCustomAttribute(() => new DescriptionAttribute(enumValue.ToString()))!;

    public static TAttribute GetCustomAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute, new()
        => enumValue.GetCustomAttribute(() => new TAttribute())!;

    public static TAttribute? GetCustomAttribute<TAttribute>(this Enum enumValue, Func<TAttribute?> defaultFunc)
        where TAttribute : Attribute
    {
        var attribute = AttributeUtils.GetCustomAttribute<TAttribute>(enumValue.GetType(), enumValue.ToString(), inherit: false);

        return attribute ?? defaultFunc();
    }
}
