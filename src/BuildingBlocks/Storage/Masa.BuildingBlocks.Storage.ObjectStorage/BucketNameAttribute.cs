// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

[AttributeUsage(AttributeTargets.Class)]
public class BucketNameAttribute : Attribute
{
    public string? Name { get; set; }

    public BucketNameAttribute(string? name = null) => Name = name;

    public static string GetName<T>() => GetName(typeof(T));

    public static string GetName(Type type)
    {
        var nameAttribute = type.GetTypeInfo().GetCustomAttribute<BucketNameAttribute>();

        if (nameAttribute == null)
            return BucketNames.DEFAULT_BUCKET_NAME;

        return !string.IsNullOrWhiteSpace(nameAttribute.Name) ? nameAttribute.Name : type.FullName!;
    }
}
