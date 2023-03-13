// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.BlazorServer;

public class AuthenticationService : IAuthenticationService
{
    private readonly TokenProvider _tokenProvider;
    private readonly string _authenticateScheme;

    public AuthenticationService(TokenProvider tokenProvider,
        string authenticateScheme)
    {
        _tokenProvider = tokenProvider;
        _authenticateScheme = authenticateScheme;
    }

    public Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        if (!_tokenProvider.Authorization.IsNullOrWhiteSpace())
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authenticateScheme, _tokenProvider.Authorization);

        return Task.CompletedTask;
    }
}
