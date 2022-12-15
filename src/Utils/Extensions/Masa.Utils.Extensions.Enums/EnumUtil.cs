// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class EnumUtil
{
    [Obsolete("Use GetCustomAttribute instead")]
    public static TAttribute? GetSubitemAttribute<TAttribute>(object? enumSubitem)
        where TAttribute : Attribute, new()
    {
        if (enumSubitem == null) return null;

        return GetCustomAttribute<TAttribute>(enumSubitem.GetType(), enumSubitem);
    }

    public static string? GetDescriptionValue<TEnum>(object? enumValue)
        where TEnum : Enum
        => GetDescriptionValue(typeof(TEnum), enumValue);

    public static string? GetDescriptionValue(Type enumType, object? enumValue)
    {
        if (enumValue == null) return null;

        return GetCustomAttribute(enumType, enumValue, name => new DescriptionAttribute(name))?
            .Description;
    }

    public static DescriptionAttribute? GetDescription<TEnum>(object? enumValue)
        where TEnum : Enum
        => GetDescription(typeof(TEnum), enumValue);

    public static DescriptionAttribute? GetDescription(Type enumType, object? enumValue)
    {
        if (enumValue == null) return null;

        return GetCustomAttribute(enumType, enumValue, name => new DescriptionAttribute(name));
    }

    public static TAttribute? GetCustomAttribute<TEnum, TAttribute>(object? enumValue)
        where TAttribute : Attribute, new()
        where TEnum : Enum
        => GetCustomAttribute(typeof(TEnum), enumValue, _ => new TAttribute());

    public static TAttribute? GetCustomAttribute<TAttribute>(Type enumType, object? enumValue)
        where TAttribute : Attribute, new()
        => GetCustomAttribute(enumType, enumValue, name => new TAttribute());

    public static TAttribute? GetCustomAttribute<TEnum, TAttribute>(object? enumValue, Func<string, TAttribute?> defaultFunc)
        where TAttribute : Attribute
        where TEnum : Enum
        => GetCustomAttribute(typeof(TEnum), enumValue, defaultFunc);

    public static TAttribute? GetCustomAttribute<TAttribute>(Type enumType, object? enumValue, Func<string, TAttribute?> defaultFunc)
        where TAttribute : Attribute
    {
        if (!enumType.IsEnum) throw new NotSupportedException();

        var value = enumValue?.ToString();
        if (value == null)
            return null;

        var name = Enum.GetName(enumType, enumValue!);
        if (name == null) return null;

        var attribute = AttributeUtils.GetCustomAttribute<TAttribute>(enumType, name, inherit: false);
        return attribute ?? defaultFunc(name);
    }

    public static Dictionary<TEnum, TAttribute> GetCustomAttributeDictionary<TEnum, TAttribute>()
        where TAttribute : Attribute, new()
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);

        return Enum.GetNames(enumType)
            .Select(name => (TEnum)Enum.Parse(enumType, name))
            .ToDictionary(e => e, e => e.GetCustomAttribute<TAttribute>());
    }

    public static IEnumerable<TEnum> GetItems<TEnum>()
        where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        return Enum.GetNames(enumType).Select(name => (TEnum)Enum.Parse(enumType, name));
    }

    public static List<EnumObject<int>> GetList<TEnum>(bool withAll = false, string allName = "所有", int allValue = default)
        where TEnum : Enum
        => GetListCore(typeof(TEnum), withAll, allName, allValue);

    public static List<EnumObject<int>> GetList(Type enumType, bool withAll = false, string allName = "所有", int allValue = default)
        => GetListCore(enumType, withAll, allName, allValue);

    public static List<EnumObject<TEnum?>> GetEnumList<TEnum>(
        bool withAll = false,
        string allName = "所有",
        TEnum? allValue = default)
        => GetListCore(typeof(TEnum), withAll, allName, allValue);

    private static List<EnumObject<TValue?>> GetListCore<TValue>(
        Type enumType,
        bool withAll = false,
        string allName = "所有",
        TValue? allValue = default)
    {
        if (!enumType.IsEnum) throw new NotSupportedException();

        var list = Enum
            .GetNames(enumType)
            .Select(name =>
            {
                var @enum = Enum.Parse(enumType, name);
                return new EnumObject<TValue?>()
                {
                    Name = GetDescriptionValue(enumType, @enum)!,
                    Value = (TValue)@enum
                };
            }).ToList();

        if (withAll)
        {
            list.Insert(0, new EnumObject<TValue?>
            {
                Name = allName,
                Value = allValue
            });
        }

        return list;
    }
}
