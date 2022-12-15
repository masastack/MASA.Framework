// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class Enum<TEnum>
    where TEnum : Enum
{
    [Obsolete("Has no effect and will be deleted")]
    public static List<TAttribute> GetAttributes<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var enumType = typeof(TEnum);
        var values = Enum.GetValues(enumType);

        var list = new List<TAttribute>();
        foreach (var value in values)
        {
            list.Add(EnumUtil.GetCustomAttribute<TAttribute>(enumType, value)!);
        }

        return list;
    }

    [Obsolete("Use EnumUtil.GetCustomAttributeDictionary<TEnum, TAttribute>() instead")]
    public static Dictionary<TEnum, TAttribute> GetDictionary<TAttribute>()
        where TAttribute : Attribute, new()
        => EnumUtil.GetCustomAttributeDictionary<TEnum, TAttribute>();

    [Obsolete("Use EnumUtil.GetItems<TEnum>() instead")]
    public static List<TEnum> GetItems() => EnumUtil.GetItems<TEnum>().ToList();

    [Obsolete("Use EnumUtil.GetEnumList<TEnum>() or EnumUtil.GetList<TEnum>() instead")]
    public static List<EnumObject<int>> GetEnumObjectList(bool withAll = false, string allName = "所有", int allValue = 0)
        => GetEnumObjectList<int>(withAll, allName, allValue);

    [Obsolete("Use EnumUtil.GetEnumList<TEnum>() or EnumUtil.GetList<TEnum>() instead")]
    public static List<EnumObject<TValue>> GetEnumObjectList<TValue>(bool withAll = false, string allName = "所有",
        TValue? allValue = default)
    {
        var enumType = typeof(TEnum);

        var lstResult = Enum.GetNames(enumType).Select(name =>
        {
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo != null)
            {
                var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(false);
                if (attribute != null)
                {
                    return new EnumObject<TValue>()
                    {
                        Name = attribute.Description,
                        Value = (TValue)Enum.Parse(enumType, name)
                    };
                }
            }
            return new EnumObject<TValue>()
            {
                Name = name,
                Value = (TValue)Enum.Parse(enumType, name)
            };
        }).ToList();

        if (withAll)
        {
            lstResult.Insert(0, new EnumObject<TValue>
            {
                Name = allName,
                Value = (TValue)Enum.Parse(enumType, allValue?.ToString() ?? string.Empty)
            });
        }

        return lstResult;
    }

    [Obsolete("Use EnumUtil.GetList<TEnum>() instead")]
    public static Dictionary<int, string> GetEnumObjectDictionary(bool withAll = false, string allName = "所有", int allValue = 0)
    {
        return GetEnumObjectDictionary<int>(withAll, allName, allValue);
    }

    [Obsolete("Use EnumUtil.GetEnumList<TEnum>() or EnumUtil.GetList<TEnum>() instead")]
    public static Dictionary<TValue, string> GetEnumObjectDictionary<TValue>(bool withAll = false, string allName = "所有",
        TValue allValue = default!)
        where TValue : notnull
    {
        Dictionary<TValue, string> keyValues = new Dictionary<TValue, string>();
        var enumType = typeof(TEnum);

        if (withAll)
        {
            keyValues.Add(allValue, allName);
        }

        foreach (var value in Enum.GetValues(enumType))
        {
            string val = value?.ToString() ?? string.Empty;
            var fieldInfo = enumType.GetField(val);
            var attribute = fieldInfo!.GetCustomAttribute<DescriptionAttribute>(false);

            keyValues.Add((TValue)Enum.Parse(enumType, val), attribute?.Description ?? val);
        }

        return keyValues;
    }
}
