// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public class SerializationBuilder
{
    public string Name { get; }

    public IServiceCollection Services { get; }

    public SerializationBuilder(string name, IServiceCollection services)
    {
        Name = name;
        Services = services;
    }
}
