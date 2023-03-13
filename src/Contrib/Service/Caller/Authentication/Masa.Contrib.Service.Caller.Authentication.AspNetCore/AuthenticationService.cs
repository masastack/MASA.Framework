// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.AspNetCore;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _authenticateScheme;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor,
        string authenticateScheme)
    {
        _httpContextAccessor = httpContextAccessor;
        _authenticateScheme = authenticateScheme;
    }

    public Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        if (AuthenticationHeaderValue.TryParse(
                _httpContextAccessor.HttpContext?.Request.Headers.Authorization,
                out var authenticationHeaderValue) && !authenticationHeaderValue.Parameter.IsNullOrWhiteSpace())
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authenticateScheme, authenticationHeaderValue.Parameter);
        }

        return Task.CompletedTask;
    }
}
