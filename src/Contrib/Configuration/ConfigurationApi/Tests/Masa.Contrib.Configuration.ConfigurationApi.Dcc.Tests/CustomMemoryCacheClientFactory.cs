// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests;

public class CustomMemoryCacheClientFactory : IMultilevelCacheClientFactory
{
    private readonly IMemoryCache _memoryCache;

    public CustomMemoryCacheClientFactory(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public IMultilevelCacheClient Create()
    {
        throw new NotImplementedException();
    }

    public IMultilevelCacheClient Create(string name)
    {
        throw new NotImplementedException();
    }

    public bool TryCreate(string name, [NotNullWhen(true)] out IMultilevelCacheClient? service)
    {
        throw new NotImplementedException();
    }
}
