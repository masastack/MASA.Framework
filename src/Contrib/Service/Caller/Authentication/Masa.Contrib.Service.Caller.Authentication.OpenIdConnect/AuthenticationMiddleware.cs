// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

public class AuthenticationMiddleware : ICallerMiddleware
{
    private readonly TokenProvider _tokenProvider;
    private readonly ITokenValidatorHandler? _tokenValidatorHandler;
    private readonly string _authenticateScheme;

    public AuthenticationMiddleware(TokenProvider tokenProvider,
        ITokenValidatorHandler? tokenValidatorHandler,
        string authenticateScheme)
    {
        _tokenProvider = tokenProvider;
        _tokenValidatorHandler = tokenValidatorHandler;
        _authenticateScheme = authenticateScheme;
    }

    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        _tokenValidatorHandler?.ValidateTokenAsync(_tokenProvider);

        if (!_tokenProvider.AccessToken.IsNullOrWhiteSpace())
            masaHttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authenticateScheme, _tokenProvider.AccessToken);

        return next();
    }
}
