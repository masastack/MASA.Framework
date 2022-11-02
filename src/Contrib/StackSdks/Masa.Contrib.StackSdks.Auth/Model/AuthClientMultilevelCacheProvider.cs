// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth.Model;

public class AuthClientMultilevelCacheProvider
{
    readonly IMultilevelCacheClientFactory _clientFactory;

    public AuthClientMultilevelCacheProvider(IMultilevelCacheClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public IMultilevelCacheClient GetMultilevelCacheClient()
    {
        return _clientFactory.Create(DEFAULT_CLIENT_NAME);
    }
}
