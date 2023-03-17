// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class CallerOptionsBuilder
{
    public IServiceCollection Services { get; }

    public string Name { get; }

    /// <summary>
    /// Used to control the life cycle of services in Client
    /// </summary>
    public ServiceLifetime? Lifetime { get; set; }

    public CallerOptionsBuilder(IServiceCollection services, string name, ServiceLifetime? lifetime = ServiceLifetime.Singleton)
    {
        Services = services;
        Name = name;
        Lifetime = lifetime;
    }
}
