// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

/// <summary>
/// Supports deserialization of all <see cref="object"/>-declared types, supporting <see langword="dynamic"/>.
/// supports serialization of all <see cref="JsonDynamicType"/>-derived types.
/// </summary>
internal sealed class DynamicObjectConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        // For simplicity in adding the converter, we use a single converter instead of two.
        return typeToConvert == typeof(object) ||
            typeof(JsonDynamicType).IsAssignableFrom(typeToConvert);
    }

    public sealed override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return new JsonDynamicString(reader.GetString() ?? string.Empty, options);
            case JsonTokenType.StartArray:
                var dynamicArray = new JsonDynamicArray(options);
                ReadList(dynamicArray, ref reader, options);
                return dynamicArray;
            case JsonTokenType.StartObject:
                var dynamicObject = new JsonDynamicObject(options);
                ReadObject(dynamicObject, ref reader, options);
                return dynamicObject;
            case JsonTokenType.False:
                return new JsonDynamicBoolean(false, options);
            case JsonTokenType.True:
                return new JsonDynamicBoolean(true, options);
            case JsonTokenType.Number:
                JsonElement jsonElement = JsonElement.ParseValue(ref reader);
                return new JsonDynamicNumber(jsonElement, options);
            case JsonTokenType.Null:
                throw new NotSupportedException(nameof(reader.TokenType));
            default:
                throw new JsonException("Unexpected token type.");
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions? options)
    {
        if (value is JsonDynamicType dynamicType)
        {
            value = dynamicType.Value;
        }

        JsonSerializer.Serialize(writer, value, options);
    }

    private void ReadList(JsonDynamicArray dynamicArray, ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            object value = Read(ref reader, typeof(object), options);
            dynamicArray.Add(value);
        }
    }

    private void ReadObject(JsonDynamicObject dynamicObject, ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        while (true)
        {
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string key = reader.GetString() ?? string.Empty;

            reader.Read();
            object? value = Read(ref reader, typeof(object), options);
            dynamicObject.Add(key, value);
        }
    }
}
