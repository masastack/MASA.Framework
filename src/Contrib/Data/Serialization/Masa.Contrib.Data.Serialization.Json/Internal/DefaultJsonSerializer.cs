// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.Serialization.Json.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Memory")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.Serialization.Json;

internal class DefaultJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions? _options;

    public DefaultJsonSerializer(JsonSerializerOptions? options = null) => _options = options;

    public string Serialize<TValue>(TValue value) => JsonSerializer.Serialize(value, _options);
}
