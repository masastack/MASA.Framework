// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.RulesEngine;

public class RulesEngineOptionsBuilder
{
    public IServiceCollection Services { get; }

    public string Name { get; }

    public RulesEngineOptionsBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
