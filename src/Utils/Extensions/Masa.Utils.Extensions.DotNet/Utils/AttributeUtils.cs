// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class AttributeUtils
{
    [Obsolete("Use GetDescriptionValueByField instead")]
    public static string? GetDescriptionByConst<TClass>(string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionValueByField(typeof(TClass), fieldName, bindingFlags);

    [Obsolete("Use GetDescriptionValueByField instead")]
    public static string? GetDescriptionByConst(Type classType, string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionValueByField(classType, fieldName, bindingFlags);

    public static string? GetDescriptionByField<TClass>(string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionValueByField(typeof(TClass), fieldName, bindingFlags);

    public static string? GetDescriptionValueByField(Type classType, string fieldName, BindingFlags? bindingFlags = null)
    {
        var fieldInfo = GetFieldInfo(classType, fieldName, bindingFlags);
        if (fieldInfo == null) return null;

        return GetDescriptionValueByField(fieldInfo);
    }

    public static string? GetDescriptionValueByField(FieldInfo fieldInfo)
        => GetCustomAttributeValue<string, DescriptionAttribute>(fieldInfo, attribute => attribute.Description);

    public static IEnumerable<TAttribute?> GetCustomAttributes<TClass, TAttribute>(
        string fieldName,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttributes<TAttribute>(typeof(TClass), fieldName, bindingFlags, inherit);

    public static IEnumerable<TAttribute?> GetCustomAttributes<TAttribute>(
        Type classType,
        string fieldName,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttributes<TAttribute>(classType, fieldName, out _, bindingFlags, inherit);

    public static IEnumerable<TAttribute?> GetCustomAttributes<TClass, TAttribute>(
        string fieldName,
        out bool existFieldInfo,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttributes<TAttribute>(typeof(TClass), fieldName,out existFieldInfo, bindingFlags, inherit);

    public static IEnumerable<TAttribute?> GetCustomAttributes<TAttribute>(
        Type classType,
        string fieldName,
        out bool existFieldInfo,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
    {
        var fieldInfo = GetFieldInfo(classType, fieldName, bindingFlags);
        existFieldInfo = fieldInfo != null;
        if (!existFieldInfo) return new List<TAttribute?>();

        return fieldInfo!.GetCustomAttributes<TAttribute>(inherit);
    }

    public static TAttribute? GetCustomAttribute<TClass, TAttribute>(
        string fieldName,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttribute<TAttribute>(typeof(TClass), fieldName, bindingFlags, inherit);

    public static TAttribute? GetCustomAttribute<TAttribute>(
        Type classType,
        string fieldName,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttribute<TAttribute>(classType, fieldName, out _, bindingFlags, inherit);

    public static TAttribute? GetCustomAttribute<TClass, TAttribute>(
        string fieldName,
        out bool existFieldInfo,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
        => GetCustomAttribute<TAttribute>(typeof(TClass), fieldName,out existFieldInfo, bindingFlags, inherit);

    public static TAttribute? GetCustomAttribute<TAttribute>(
        Type classType,
        string fieldName,
        out bool existFieldInfo,
        BindingFlags? bindingFlags = null,
        bool inherit = true)
        where TAttribute : Attribute
    {
        var fieldInfo = GetFieldInfo(classType, fieldName, bindingFlags);
        existFieldInfo = fieldInfo != null;
        if (!existFieldInfo) return null;

        return fieldInfo!.GetCustomAttribute<TAttribute>(inherit);
    }

    public static TOpt? GetCustomAttributeValue<TOpt, TAttribute>(
        FieldInfo fieldInfo,
        Func<TAttribute, TOpt> valueSelector,
        bool inherit = true)
        where TAttribute : Attribute
    {
        var attribute = fieldInfo.GetCustomAttribute<TAttribute>(inherit);
        return attribute == null ? default : valueSelector(attribute);
    }

    private static FieldInfo? GetFieldInfo(Type type, string fieldName, BindingFlags? bindingFlags = null)
        => type.GetField(fieldName, bindingFlags ?? BindingFlags.Public | BindingFlags.Static);
}
