// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class DefaultDaprClientBuilder
{
    public IServiceCollection Services { get; private set; }

    public string Name { get; private set; }

    public DefaultDaprClientBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
