// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Caller;

public class AuthenticationService : IAuthenticationService
{
    private readonly TokenProvider _tokenProvider;
    private readonly JwtTokenValidator? _jwtTokenValidator;
    private readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public AuthenticationService(TokenProvider tokenProvider,
        JwtTokenValidator? jwtTokenValidator,
        IMultiEnvironmentContext multiEnvironmentContext)
    {
        _tokenProvider = tokenProvider;
        _jwtTokenValidator = jwtTokenValidator;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    public async Task ExecuteAsync(HttpRequestMessage requestMessage)
    {
        if (_jwtTokenValidator != null)
        {
            await _jwtTokenValidator.ValidateTokenAsync(_tokenProvider);
        }

        if (!_tokenProvider.AccessToken.IsNullOrWhiteSpace())
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);

        if (!string.IsNullOrEmpty(_multiEnvironmentContext.CurrentEnvironment) && !requestMessage.Headers.Any(x=>x.Key == IsolationConsts.ENVIRONMENT))
            requestMessage.Headers.Add(IsolationConsts.ENVIRONMENT, _multiEnvironmentContext.CurrentEnvironment);
    }
}
