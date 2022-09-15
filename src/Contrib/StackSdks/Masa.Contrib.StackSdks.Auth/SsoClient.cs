// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class SsoClient : ISsoClient
{
    public IThirdPartyIdpCacheService ThirdPartyIdpCacheService { get; }

    public SsoClient(IThirdPartyIdpCacheService thirdPartyIdpCacheService)
    {
        ThirdPartyIdpCacheService = thirdPartyIdpCacheService;
    }
}

