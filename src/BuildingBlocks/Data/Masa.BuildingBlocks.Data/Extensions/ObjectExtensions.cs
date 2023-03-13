// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public static class ObjectExtensions
{
    public static string ToJson(this object? obj)
        => obj.ToJson(null);

    public static string ToJson(this object? obj, JsonSerializerOptions? options)
    {
        if (obj == null)
            return string.Empty;

        var jsonSerializer = MasaApp.GetService<IJsonSerializer>();
        if (jsonSerializer != null)
            return jsonSerializer.Serialize(obj);

        return JsonSerializer.Serialize(obj, options);
    }

    public static string ToYaml(this object obj)
    {
        var yamlSerializer = MasaApp.GetService<IYamlSerializer>();
        if (yamlSerializer == null)
            throw new NotSupportedException("Please check whether to inject yaml serialization and deserialization components");

        return yamlSerializer.Serialize(obj);
    }
}
