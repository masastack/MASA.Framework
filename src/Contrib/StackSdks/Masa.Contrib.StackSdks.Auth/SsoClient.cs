// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class SsoClient : ISsoClient
{
    public SsoClient(IHttpClientFactory httpClientFactory)
    {
        LoginService =  new LoginService(httpClientFactory);
    }

    public ILoginService LoginService { get; set; }
}

