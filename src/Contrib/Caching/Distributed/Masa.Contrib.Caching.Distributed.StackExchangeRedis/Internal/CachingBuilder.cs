// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal class CachingBuilder : ICachingBuilder
{
    public IServiceCollection Services { get; }
    
    public string Name { get; }

    public CachingBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
