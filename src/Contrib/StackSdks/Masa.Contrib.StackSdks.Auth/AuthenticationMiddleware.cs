// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class AuthenticationMiddleware : ICallerMiddleware
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationMiddleware(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        var tokenProvider = _httpContextAccessor.HttpContext?.RequestServices.GetService<TokenProvider>();
        if (tokenProvider != null)
            masaHttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);

        var environment = _httpContextAccessor.HttpContext?.RequestServices.GetService<IEnvironmentProvider>();
        if (environment != null)
            masaHttpContext.RequestMessage.Headers.Add(IsolationConsts.ENVIRONMENT, environment.GetEnvironment());

        return next.Invoke();
    }
}
