// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class AttributeUtils
{
    [Obsolete("Use GetDescriptionByField instead")]
    public static string? GetDescriptionByConst<TClass>(string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionByField(typeof(TClass), fieldName, bindingFlags);

    [Obsolete("Use GetDescriptionByField instead")]
    public static string? GetDescriptionByConst(Type classType, string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionByField(classType, fieldName, bindingFlags);

    public static string? GetDescriptionByField<TClass>(string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionByField(typeof(TClass), fieldName, bindingFlags);

    public static string? GetDescriptionByField(Type classType, string fieldName, BindingFlags? bindingFlags = null)
    {
        var fieldInfo = GetFieldInfo(classType, fieldName, bindingFlags);
        if (fieldInfo == null) return null;

        return GetDescriptionByField(fieldInfo);
    }

    public static string? GetDescriptionByField(FieldInfo fieldInfo)
        => GetCustomAttributeValue<string, DescriptionAttribute>(fieldInfo, attribute => attribute.Description);

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
    {
        var fieldInfo = GetFieldInfo(classType, fieldName, bindingFlags);
        if (fieldInfo == null) return null;

        return fieldInfo.GetCustomAttribute<TAttribute>(inherit);
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
