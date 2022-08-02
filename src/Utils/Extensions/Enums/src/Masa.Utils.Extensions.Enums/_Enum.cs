// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public class Enum<TEnum>
    where TEnum : Enum
{
    public static List<TAttribute> GetAttributes<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var enumType = typeof(TEnum);
        var names = Enum.GetNames(enumType);
        var result = names.Select(name => EnumUtil.GetSubitemAttribute<TAttribute>(Enum.Parse(enumType, name))!).ToList();

        return result;
    }

    public static Dictionary<TEnum, TAttribute> GetDictionary<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var enumType = typeof(TEnum);

        return Enum.GetNames(enumType)
            .Select(name => (TEnum)Enum.Parse(enumType, name))
            .ToDictionary(@enum => @enum, @enum => EnumUtil.GetSubitemAttribute<TAttribute>(@enum)!);
    }

    public static List<TEnum> GetItems()
    {
        var enumType = typeof(TEnum);

        return Enum.GetNames(enumType)
            .Select(name => (TEnum)Enum.Parse(enumType, name)).ToList();
    }

    public static List<EnumObject<int>> GetEnumObjectList(bool withAll = false, string allName = "所有", int allValue = 0)
        => GetEnumObjectList<int>(withAll, allName, allValue);

    public static List<EnumObject<TValue>> GetEnumObjectList<TValue>(bool withAll = false, string allName = "所有", TValue? allValue = default)
    {
        var enumType = typeof(TEnum);

        var lstResult = Enum.GetNames(enumType).Select(name =>
        {
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo != null)
            {
                var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(false);
                if (attribute == null)
                {
                    return new EnumObject<TValue>()
                    {
                        Name = name,
                        Value = (TValue)Enum.Parse(enumType, name)
                    };
                }
                else
                {
                    return new EnumObject<TValue>()
                    {
                        Name = attribute.Description,
                        Value = (TValue)Enum.Parse(enumType, name)
                    };
                }
            }
            else
            {
                return new EnumObject<TValue>()
                {
                    Name = name,
                    Value = (TValue)Enum.Parse(enumType, name)
                };
            }
        }).Where(p => p != null).ToList();

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

    public static Dictionary<int, string> GetEnumObjectDictionary(bool withAll = false, string allName = "所有", int allValue = 0)
    {
        return GetEnumObjectDictionary<int>(withAll, allName, allValue);
    }

    public static Dictionary<TValue, string> GetEnumObjectDictionary<TValue>(bool withAll = false, string allName = "所有", TValue allValue = default!)
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
