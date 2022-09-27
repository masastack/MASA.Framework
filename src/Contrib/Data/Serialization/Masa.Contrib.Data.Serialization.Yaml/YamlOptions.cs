// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Serialization.Yaml;

public class YamlOptions
{
    public YamlDotNet.Serialization.ISerializer Serializer { get; set; }

    public YamlDotNet.Serialization.IDeserializer Deserializer { get; set; }
}
