// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.BuildingBlocks.Data.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.Serialization.Json.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Memory")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.Serialization.Json;

internal class DefaultJsonDeserializer : IJsonDeserializer
{
    private readonly JsonSerializerOptions? _options;

    public DefaultJsonDeserializer(JsonSerializerOptions? options = null) => _options = options;

    public TValue? Deserialize<TValue>(string value)
        => JsonSerializer.Deserialize<TValue>(value, _options);

    public object? Deserialize(string value, Type valueType)
        => JsonSerializer.Deserialize(value, valueType, _options);
}
