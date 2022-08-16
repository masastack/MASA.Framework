// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache;

public class MemoryCacheProvider
{
    readonly IMemoryCacheClientFactory _memoryCacheClientFactory;

    public MemoryCacheProvider(IMemoryCacheClientFactory memoryCacheClientFactory)
    {
        _memoryCacheClientFactory = memoryCacheClientFactory;
    }

    public IMemoryCacheClient GetMemoryCacheClient()
    {
        return _memoryCacheClientFactory.CreateClient(Constants.DEFAULT_CLIENT_NAME);
    }
}
