// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public class AuthenticationService : IAuthenticationService
{
    private readonly TokenProvider _tokenProvider;
    private readonly JwtTokenValidator _jwtTokenValidator;

    public AuthenticationService(TokenProvider tokenProvider,
        JwtTokenValidator jwtTokenValidator)
    {
        _tokenProvider = tokenProvider;
        _jwtTokenValidator = jwtTokenValidator;
    }

    public async Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        await _jwtTokenValidator.ValidateTokenAsync(_tokenProvider);

        if (!_tokenProvider.AccessToken.IsNullOrWhiteSpace())
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
    }
}
