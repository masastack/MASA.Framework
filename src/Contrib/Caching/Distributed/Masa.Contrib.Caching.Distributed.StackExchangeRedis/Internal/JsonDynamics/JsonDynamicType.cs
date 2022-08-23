// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// The base class for all dynamic types supported by the serializer.
/// </summary>
internal abstract class JsonDynamicType : DynamicObject
{
    public JsonSerializerOptions Options { get; private set; }

    private protected JsonDynamicType(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        Options = options;
    }

    public sealed override bool TryConvert(ConvertBinder binder, out object? result)
    {
        return TryConvert(binder.ReturnType, out result);
    }

    public abstract T? GetValue<T>();
    public abstract void SetValue(object value);
    protected abstract bool TryConvert(Type returnType, out object? result);

    protected static bool TryConvertWithTypeConverter(object value, Type returnType, out object? result)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(value.GetType());
        if (converter.CanConvertTo(returnType))
        {
            result = converter.ConvertTo(value, returnType)!;
            return true;
        }

        converter = TypeDescriptor.GetConverter(returnType);
        if (converter.CanConvertFrom(value.GetType()))
        {
            result = converter.ConvertFrom(value)!;
            return true;
        }

        result = null;
        return false;
    }

    internal abstract object Value { get; }
}
