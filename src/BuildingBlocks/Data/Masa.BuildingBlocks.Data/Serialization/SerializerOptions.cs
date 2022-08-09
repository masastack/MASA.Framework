// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class SerializerOptions
{
    private List<SerializerOptionsBuilder> Serializers { get; set; } = new();

    private List<DeserializerOptionsBuilder> Deserializers { get; set; } = new();

    public SerializerOptions MappingSerializer(string name, Func<IServiceProvider, ISerializer> func)
    {
        var builder = Serializers.FirstOrDefault(b => b.Name == name.ToLower());
        if (builder != null)
        {
            builder.Func = func;
        }
        else
        {
            Serializers.Add(new SerializerOptionsBuilder(name.ToLower(), func));
        }
        return this;
    }

    public SerializerOptions MappingDeserializer(string name, Func<IServiceProvider, IDeserializer> func)
    {
        var builder = Deserializers.FirstOrDefault(b => b.Name == name.ToLower());
        if (builder != null)
        {
            builder.Func = func;
        }
        else
        {
            Deserializers.Add(new DeserializerOptionsBuilder(name.ToLower(), func));
        }
        return this;
    }

    public Func<IServiceProvider, ISerializer>? GetSerializer(string? name = null)
    {
        if (name == null)
        {
            var serializer = Serializers.FirstOrDefault(b => b.Name == Microsoft.Extensions.Options.Options.DefaultName) ?? Serializers.FirstOrDefault();
            return serializer?.Func;
        }
        else
        {
            var serializer = Serializers.FirstOrDefault(b => b.Name == name.ToLower());
            return serializer?.Func;
        }
    }


    public Func<IServiceProvider, IDeserializer>? GetDeserializer(string? name = null)
    {
        if (name == null)
        {
            var deserializer = Deserializers.FirstOrDefault(b => b.Name == Microsoft.Extensions.Options.Options.DefaultName) ?? Deserializers.FirstOrDefault();
            return deserializer?.Func;
        }
        else
        {
            var deserializer = Deserializers.FirstOrDefault(b => b.Name == name.ToLower());
            return deserializer?.Func;
        }
    }
}
