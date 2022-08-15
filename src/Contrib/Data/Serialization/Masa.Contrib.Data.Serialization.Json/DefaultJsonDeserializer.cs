// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Json;

public class DefaultJsonDeserializer : IJsonDeserializer
{
    private readonly IOptions<JsonSerializerOptions>? _options;

    public DefaultJsonDeserializer(IOptions<JsonSerializerOptions>? options = null) => _options = options;

    public TValue? Deserialize<TValue>(string value)
        => JsonSerializer.Deserialize<TValue>(value, _options?.Value);

    public object? Deserialize(string value, Type valueType)
        => JsonSerializer.Deserialize(value, valueType, _options?.Value);
}
