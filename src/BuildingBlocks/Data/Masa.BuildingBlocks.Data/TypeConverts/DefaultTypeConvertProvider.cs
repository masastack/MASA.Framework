// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data;

public class DefaultTypeConvertProvider : ITypeConvertProvider
{
    private readonly List<Type> _types = new()
    {
        typeof(Guid),
        typeof(Guid?),
        typeof(DateTime),
        typeof(DateTime?)
    };

    private readonly List<Type> _notNeedSerializeTypes = new()
    {
        typeof(String),
        typeof(Guid),
        typeof(DateTime),
        typeof(Decimal),
        typeof(Guid?),
        typeof(DateTime?),
        typeof(Decimal?)
    };

    private readonly IDeserializer? _deserializer;

    public DefaultTypeConvertProvider(IDeserializer? deserializer = null) => _deserializer = deserializer;

    public T? ConvertTo<T>(string value, IDeserializer? deserializer = null)
    {
        var result = ConvertTo(value, typeof(T), deserializer);
        return result is T ? (T)result : default;
    }

    public object? ConvertTo(string value, Type type, IDeserializer? deserializer = null)
    {
        if (_types.Contains(type))
            return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value)!;

        if (!IsSupportDeserialize(type))
            return Convert.ChangeType(value, type);

        return (deserializer ?? _deserializer)!.Deserialize(value, type);
    }

    private bool IsSupportDeserialize(Type type)
        => !type.IsPrimitive && !_notNeedSerializeTypes.Contains(type);
}
