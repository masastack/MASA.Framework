// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports dynamic booleans.
/// </summary>
internal sealed class JsonDynamicBoolean : JsonDynamicType
{
    private object _value;
    private Type? _type;

    public JsonDynamicBoolean(bool value, JsonSerializerOptions options) : base(options)
    {
        _value = value;
        _type = typeof(bool);
    }

    public override T? GetValue<T>() where T : default
    {
        bool success = TryConvert(typeof(T), out object? result);
        Debug.Assert(success);
        return (T)result!;
    }

    public override void SetValue(object value)
    {
        _value = value;
        _type = value.GetType();
    }

    protected override bool TryConvert(Type returnType, out object? result)
    {
        if (returnType == _value!.GetType())
        {
            result = _value; // Return cached value.
            return true;
        }

        if (TryConvertWithTypeConverter(_value, returnType, out result))
        {
            return true;
        }

        result = _value = JsonSerializer.Deserialize($"\"{Value}\"", returnType, Options)!;
        _type = result?.GetType()!;
        return true;
    }

    internal override object Value => _value;

    public static implicit operator bool(JsonDynamicBoolean obj)
    {
        bool success = obj.TryConvert(typeof(bool), out object? result);
        Debug.Assert(success);
        return (bool)result!;
    }
}
