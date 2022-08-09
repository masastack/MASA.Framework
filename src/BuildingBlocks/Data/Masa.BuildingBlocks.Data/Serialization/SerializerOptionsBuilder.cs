// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class SerializerOptionsBuilder
{
    public string Name { get; set; }

    public Func<IServiceProvider, ISerializer> Func { get; set; }

    public SerializerOptionsBuilder() { }

    public SerializerOptionsBuilder(string name, Func<IServiceProvider, ISerializer> func)
    {
        Name = name;
        Func = func;
    }
}
