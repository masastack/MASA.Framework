// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultYamlDeserializer : IYamlDeserializer
{
    private readonly YamlDotNet.Serialization.IDeserializer _deserializer;

    public DefaultYamlDeserializer(YamlDotNet.Serialization.IDeserializer deserializer) => _deserializer = deserializer;

    public TValue? Deserialize<TValue>(string value)
        => _deserializer.Deserialize<TValue>(value);

    public object? Deserialize(string value, Type valueType)
        => _deserializer.Deserialize(value, valueType);
}
