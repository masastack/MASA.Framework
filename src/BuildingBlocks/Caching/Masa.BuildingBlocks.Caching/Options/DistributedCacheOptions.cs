// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class DistributedCacheOptions
{
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name of the client configured by this builder.
    /// </summary>
    public string Name { get; }

    public DistributedCacheOptions(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
