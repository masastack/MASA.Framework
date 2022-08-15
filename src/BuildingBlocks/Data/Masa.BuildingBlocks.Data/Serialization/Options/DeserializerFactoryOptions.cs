// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DeserializerFactoryOptions : MasaFactoryOptions<DeserializerRelationOptions>
{
    public DeserializerFactoryOptions MappingDeserializer(string name, Func<IServiceProvider, IDeserializer> func)
    {
        var builder = Options.FirstOrDefault(b => b.Name == name.ToLower());
        if (builder != null) builder.Func = func;
        else Options.Add(new DeserializerRelationOptions(name.ToLower(), func));
        return this;
    }

    public Func<IServiceProvider, IDeserializer>? GetDeserializer()
        => GetDeserializer(Microsoft.Extensions.Options.Options.DefaultName);

    public Func<IServiceProvider, IDeserializer>? GetDeserializer(string name)
    {
        var deserializer = Options.FirstOrDefault(b => b.Name == name.ToLower());
        return deserializer?.Func;
    }
}
