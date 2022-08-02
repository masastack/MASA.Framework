// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

[AttributeUsage(AttributeTargets.Class)]
public class ConnectionStringNameAttribute : Attribute
{
    public string Name { get; set; }

    public ConnectionStringNameAttribute(string name) => Name = name;

    public static string GetConnStringName<T>() => GetConnStringName(typeof(T));

    public static string GetConnStringName(Type type)
    {
        var nameAttribute = type.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>();

        if (nameAttribute == null)
            return ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME;

        return !string.IsNullOrEmpty(nameAttribute.Name) ? nameAttribute.Name : type.FullName!;
    }
}
