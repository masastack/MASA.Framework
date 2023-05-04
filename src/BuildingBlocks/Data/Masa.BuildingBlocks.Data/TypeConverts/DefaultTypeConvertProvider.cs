// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class DefaultTypeConvertProvider : ITypeConvertProvider
{
    private readonly List<Type> _types = new()
    {
        typeof(Guid),
        typeof(Guid?),
        typeof(DateTime),
        typeof(DateTime?)
    };

    private readonly List<Type> _simpleTypes = new()
    {
        typeof(string),
        typeof(short?),
        typeof(short),
        typeof(int),
        typeof(int?),
        typeof(long),
        typeof(long?),
        typeof(float),
        typeof(float?),
        typeof(decimal),
        typeof(decimal?),
        typeof(double),
        typeof(double?),
        typeof(bool),
        typeof(bool?)
    };

    private readonly IDeserializer? _deserializer;

    public DefaultTypeConvertProvider(IDeserializer? deserializer = null) => _deserializer = deserializer;

    public T? ConvertTo<T>(string value, IDeserializer? deserializer = null)
    {
        var result = ConvertTo(value, typeof(T), deserializer);
        return result is T res ? res : default;
    }

    public object? ConvertTo(string value, Type type, IDeserializer? deserializer = null)
    {
        if (value.IsNullOrWhiteSpace())
            return default;

        if (_simpleTypes.Contains(type))
        {
            if (type.IsNullableType())
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type)!);

            return Convert.ChangeType(value, type);
        }

        if (_types.Contains(type))
            return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)!;

        return (deserializer ?? _deserializer)!.Deserialize(value, type);
    }
}
