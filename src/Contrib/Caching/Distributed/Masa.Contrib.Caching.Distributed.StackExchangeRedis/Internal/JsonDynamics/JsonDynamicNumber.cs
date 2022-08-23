// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports dynamic numbers.
/// </summary>
internal sealed class JsonDynamicNumber : JsonDynamicType
{
    private Type? _type;
    private object _value;
    private object? _lastValue = null;

    public JsonDynamicNumber(object? value, JsonSerializerOptions options) : base(options)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override T? GetValue<T>() where T : default
    {
        if (TryConvert(typeof(T), out object? result))
            return result is T ? (T)result : default;

        throw new InvalidOperationException($"Cannot change type {_value.GetType()} to {typeof(T)}.");
    }

    public override void SetValue(object value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        _value = _lastValue = value;
        _type = value.GetType();
    }

    protected override bool TryConvert(Type returnType, out object? result)
    {
        if (returnType == _type)
        {
            result = _lastValue; // Return cached value, such as a long or double.
            return true;
        }

        bool success = false;
        result = null;

        if (!(_value is JsonElement jsonElement))
        {
            return TryConvertWithTypeConverter(_value, returnType, out result);
        }

        if (returnType == typeof(long))
        {
            success = jsonElement.TryGetInt64(out long value);
            result = value;
        }
        else if (returnType == typeof(double))
        {
            success = jsonElement.TryGetDouble(out double value);
            result = value;
        }
        else if (returnType == typeof(int))
        {
            success = jsonElement.TryGetInt32(out int value);
            result = value;
        }
        else if (returnType == typeof(short))
        {
            success = jsonElement.TryGetInt16(out short value);
            result = value;
        }
        else if (returnType == typeof(decimal))
        {
            success = jsonElement.TryGetDecimal(out decimal value);
            result = value;
        }
        else if (returnType == typeof(byte))
        {
            success = jsonElement.TryGetByte(out byte value);
            result = value;
        }
        else if (returnType == typeof(float))
        {
            success = jsonElement.TryGetSingle(out float value);
            result = value;
        }
        else if (returnType == typeof(uint))
        {
            success = jsonElement.TryGetUInt32(out uint value);
            result = value;
        }
        else if (returnType == typeof(ushort))
        {
            success = jsonElement.TryGetUInt16(out ushort value);
            result = value;
        }
        else if (returnType == typeof(ulong))
        {
            success = jsonElement.TryGetUInt64(out ulong value);
            result = value;
        }
        else if (returnType == typeof(sbyte))
        {
            success = jsonElement.TryGetSByte(out sbyte value);
            result = value;
        }

        if (!success)
        {
            // Use the raw test which may be recognized by converters such as the Enum converter than can process numbers.
            string rawText = jsonElement.GetRawText();
            result = JsonSerializer.Deserialize($"{rawText}", returnType, Options)!;
        }

        _lastValue = result!;
        _type = result!.GetType();
        return true;
    }

    internal override object Value => _value;
}
