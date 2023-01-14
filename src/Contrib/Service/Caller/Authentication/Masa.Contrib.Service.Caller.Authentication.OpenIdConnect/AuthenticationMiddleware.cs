// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;

public class AuthenticationMiddleware : ICallerMiddleware
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _authenticateScheme;

    public AuthenticationMiddleware(IHttpContextAccessor httpContextAccessor,
        string authenticateScheme)
    {
        _httpContextAccessor = httpContextAccessor;
        _authenticateScheme = authenticateScheme;
    }

    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        var serviceProvider = GetServiceProvider();
        MasaArgumentException.ThrowIfNull(serviceProvider);

        var tokenValidatorHandler = serviceProvider.GetService<ITokenValidatorHandler>();
        var tokenProvider = serviceProvider.GetRequiredService<TokenProvider>();

        tokenValidatorHandler?.ValidateTokenAsync(tokenProvider);

        if (!tokenProvider.AccessToken.IsNullOrWhiteSpace())
            masaHttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authenticateScheme, tokenProvider.AccessToken);

        return next();
    }

    private IServiceProvider? GetServiceProvider() => _httpContextAccessor.HttpContext?.RequestServices;
}
