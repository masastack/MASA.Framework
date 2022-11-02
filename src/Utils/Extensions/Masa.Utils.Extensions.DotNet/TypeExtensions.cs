// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class TypeExtensions
{
    public static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    public static string GetGenericTypeName(this object @object)
        => @object.GetType().GetGenericTypeName();

    public static bool IsNullableType(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Determine if a class is derived from a generic class
    /// todo: The restrictions on type and genericType will be removed in the future
    /// </summary>
    /// <param name="type">common class, Generic classes are not supported</param>
    /// <param name="genericType">generic class, Only interface generics are supported</param>
    /// <returns></returns>
    public static bool IsImplementerOfGeneric(this Type type, Type genericType)
    {
        if (!genericType.GetTypeInfo().IsGenericType || !type.IsClass)
            return false;

        return type.GetInterfaces().Any(interfaceType =>
        {
            var current = interfaceType.GetTypeInfo().IsGenericType ?
                interfaceType.GetGenericTypeDefinition() : interfaceType;
            return current == genericType;
        });
    }
}
