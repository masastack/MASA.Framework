// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

[AttributeUsage(AttributeTargets.Class)]
public class ConnectionStringNameAttribute : Attribute
{
    public string Name { get; set; }

    public ConnectionStringNameAttribute(string name = "") => Name = name;

    private static bool _isUseDefaultConnectionStringName;
    private static readonly List<DbContextNameRelationOptions> DbContextNameRelationOptions = new();

    public static string GetConnStringName<T>() => GetConnStringName(typeof(T));

    public static string GetConnStringName(Type type)
    {
        var options = DbContextNameRelationOptions.FirstOrDefault(c => c.DbContextType == type);
        if (options != null) return options.Name;

        var nameAttribute = type.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>();

        var name = nameAttribute?.Name;
        if (name.IsNullOrWhiteSpace())
        {
            if (_isUseDefaultConnectionStringName) name = type.FullName;
            else
            {
                _isUseDefaultConnectionStringName = true;
                name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME;
            }
        }
        DbContextNameRelationOptions.Add(new DbContextNameRelationOptions(name!, type));
        return name!;
    }
}
