// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DefaultYamlSerializer : IYamlSerializer
{
    private readonly YamlDotNet.Serialization.ISerializer _serializer;

    public DefaultYamlSerializer(YamlDotNet.Serialization.ISerializer serializer) => _serializer = serializer;

    public string Serialize<TValue>(TValue value) => value == null ? string.Empty : _serializer.Serialize(value);
}
