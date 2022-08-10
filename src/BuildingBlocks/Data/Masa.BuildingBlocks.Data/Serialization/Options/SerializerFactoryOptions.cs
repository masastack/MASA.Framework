// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class SerializerFactoryOptions : MasaFactoryOptions<SerializerRelationOptions>
{
    public SerializerFactoryOptions MappingSerializer(string name, Func<IServiceProvider, ISerializer> func)
    {
        var builder = Options.FirstOrDefault(b => b.Name == name.ToLower());
        if (builder != null) builder.Func = func;
        else Options.Add(new SerializerRelationOptions(name.ToLower(), func));
        return this;
    }

    public Func<IServiceProvider, ISerializer>? GetSerializer()
        => GetSerializer(Microsoft.Extensions.Options.Options.DefaultName);

    public Func<IServiceProvider, ISerializer>? GetSerializer(string name)
    {
        var serializer = Options.FirstOrDefault(b => b.Name == name.ToLower());
        return serializer?.Func;
    }
}
