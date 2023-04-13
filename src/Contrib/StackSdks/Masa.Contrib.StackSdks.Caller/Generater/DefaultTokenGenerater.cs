// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

internal class DefaultTokenGenerater : ITokenGenerater
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string SCHEME = "Bearer ";

    public DefaultTokenGenerater(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TokenProvider Generater()
    {
        StringValues authenticationHeaderValue;

        if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out authenticationHeaderValue) == true)
        {
            var accessToken = authenticationHeaderValue.ToString();

            if (!string.IsNullOrEmpty(accessToken) && accessToken.ToString().IndexOf(SCHEME) > -1)
            {
                accessToken = accessToken.Replace(SCHEME, "");
            }

            return new TokenProvider { AccessToken = accessToken };
        }

        return new TokenProvider();
    }
}
