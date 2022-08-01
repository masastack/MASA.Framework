// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.Collections.Concurrent;

public static class ExtraPropertiesExtensions
{
    public static bool HasProperty(this ExtraPropertyDictionary source, string name)
    {
        return source.ContainsKey(name);
    }

    public static object GetProperty(this ExtraPropertyDictionary source, string name, object defaultValue = null)
    {
        return source.GetOrDefault(name)
               ?? defaultValue;
    }

    public static TProperty GetProperty<TProperty>(this ExtraPropertyDictionary source, string name, TProperty defaultValue = default)
    {
        var value = source.GetProperty(name);
        if (value == null)
        {
            return defaultValue;
        }

        if (TypeHelper.IsPrimitiveExtended(typeof(TProperty), includeEnums: true))
        {
            var conversionType = typeof(TProperty);
            if (TypeHelper.IsNullable(conversionType))
            {
                conversionType = conversionType.GetFirstGenericArgumentIfNullable();
            }

            if (conversionType == typeof(Guid))
            {
                return (TProperty)TypeDescriptor.GetConverter(conversionType).ConvertFromInvariantString(value.ToString());
            }

            if (conversionType == typeof(DateTimeOffset))
            {
                return (TProperty)TypeDescriptor.GetConverter(conversionType).ConvertFromInvariantString(value.ToString());
            }

            return (TProperty)Convert.ChangeType(value?.ToString(), conversionType, CultureInfo.InvariantCulture);
        }

        throw new Exception("GetProperty<TProperty> does not support non-primitive types. Use non-generic GetProperty method and handle type casting manually.");
    }

    public static TSource SetProperty<TSource>(
        this TSource source,
        string name,
        object value)
        where TSource : ExtraPropertyDictionary
    {
        source[name] = value;

        return source;
    }
}
