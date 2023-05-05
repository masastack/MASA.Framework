// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.Serialization.Json")]
[assembly: InternalsVisibleTo("Masa.Contrib.Data.Serialization.Yaml")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

internal static class SerializationBuilderExtensions
{
    public static void UseSerialization(
        this SerializationBuilder serializationBuilder,
        Func<IServiceProvider, ISerializer> serializerFunc,
        Func<IServiceProvider, IDeserializer> deserializerFunc)
    {
        serializationBuilder.Services.Configure<SerializerFactoryOptions>(
            options => options.TryAdd(serializationBuilder.Name, serializerFunc));

        serializationBuilder.Services.Configure<DeserializerFactoryOptions>(
            options => options.TryAdd(serializationBuilder.Name, deserializerFunc));
    }
}
