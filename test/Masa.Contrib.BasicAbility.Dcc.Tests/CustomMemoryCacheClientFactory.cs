// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Dcc.Tests;

public class CustomMemoryCacheClientFactory : IMemoryCacheClientFactory
{
    private readonly IMemoryCache _memoryCache;

    public CustomMemoryCacheClientFactory(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public MemoryCacheClient CreateClient(string name) => new(_memoryCache, null!, SubscribeKeyTypes.SpecificPrefix);
}
