// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class AttributeUtils
{
    public static string? GetDescriptionByConst<TClass>(string fieldName, BindingFlags? bindingFlags = null)
        => GetDescriptionByConst(typeof(TClass), fieldName, bindingFlags);

    public static string? GetDescriptionByConst(Type type, string fieldName, BindingFlags? bindingFlags = null)
    {
        var fieldInfo = type.GetField(fieldName, bindingFlags ?? BindingFlags.Public | BindingFlags.Static);
        if (fieldInfo == null)
            return null;

        return GetDescriptionByField(fieldInfo);
    }

    public static string? GetDescriptionByField(FieldInfo fieldInfo)
        => GetFieldAttributeValue<string, DescriptionAttribute>(fieldInfo, attribute => attribute.Description);

    public static TOpt? GetFieldAttributeValue<TOpt, TAttribute>(
        FieldInfo fieldInfo,
        Func<TAttribute, TOpt> valueSelector)
        where TAttribute : Attribute
    {
        return fieldInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att ? valueSelector(att) : default;
    }
}
