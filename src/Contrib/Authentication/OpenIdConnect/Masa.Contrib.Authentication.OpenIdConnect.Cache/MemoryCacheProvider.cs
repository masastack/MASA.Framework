// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache;

public class MemoryCacheProvider
{
    readonly IMultilevelCacheClientFactory _clientFactory;

    public MemoryCacheProvider(IMultilevelCacheClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IMultilevelCacheClient GetMemoryCacheClient()
    {
        return _clientFactory.Create(Constants.DEFAULT_CLIENT_NAME);
    }
}
