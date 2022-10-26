// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.RulesEngine;

public class RulesEngineOptions
{
    public IServiceCollection Services { get; }

    public RulesEngineOptions(IServiceCollection services)
    {
        Services = services;
    }
}
