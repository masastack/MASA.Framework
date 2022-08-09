// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class DeserializerOptionsBuilder
{
    public string Name { get; set; }

    public Func<IServiceProvider, IDeserializer> Func { get; set; }

    public DeserializerOptionsBuilder() { }

    public DeserializerOptionsBuilder(string name, Func<IServiceProvider, IDeserializer> func)
    {
        Name = name;
        Func = func;
    }
}
