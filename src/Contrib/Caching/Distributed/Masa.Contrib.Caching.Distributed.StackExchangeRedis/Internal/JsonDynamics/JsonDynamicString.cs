// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports dynamic strings.
/// </summary>
internal sealed class JsonDynamicString : JsonDynamicType
{
    private object _value;
    private Type? _type;

    public JsonDynamicString(string value, JsonSerializerOptions options) : base(options)
    {
        _value = value;
        _type = typeof(string);
    }

    public override T? GetValue<T>() where T : default
    {
        bool success = TryConvert(typeof(T), out object? result);
#if DEBUG
        Debug.Assert(success);
#endif
        return result is T ? (T)result : default;
    }

    public override void SetValue(object value)
    {
        _value = value;
        _type = value.GetType();
    }

    protected override bool TryConvert(Type returnType, out object? result)
    {
        if (returnType == _type)
        {
            result = _value; // Return cached value, such as a DateTime.
            return true;
        }

        if (TryConvertWithTypeConverter(_value, returnType, out result))
            return true;

        result = _value = JsonSerializer.Deserialize($"\"{_value}\"", returnType, Options)!;
        _type = result.GetType();
        return true;
    }

    internal override object Value => _value;

    public static implicit operator string(JsonDynamicString obj)
    {
        bool success = obj.TryConvert(typeof(string), out object? result);
        Debug.Assert(success);
        return (string)result!;
    }
}
