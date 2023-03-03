// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

public class AuthenticationService : IAuthenticationService
{
    private readonly TokenProvider _tokenProvider;
    private readonly ITokenValidatorHandler? _tokenValidatorHandler;
    private readonly string _authenticateScheme;

    public AuthenticationService(TokenProvider tokenProvider,
        ITokenValidatorHandler? tokenValidatorHandler,
        string authenticateScheme)
    {
        _tokenProvider = tokenProvider;
        _tokenValidatorHandler = tokenValidatorHandler;
        _authenticateScheme = authenticateScheme;
    }

    public Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        _tokenValidatorHandler?.ValidateTokenAsync(_tokenProvider);

        if (!_tokenProvider.AccessToken.IsNullOrWhiteSpace())
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authenticateScheme, _tokenProvider.AccessToken);

        return Task.CompletedTask;
    }
}
