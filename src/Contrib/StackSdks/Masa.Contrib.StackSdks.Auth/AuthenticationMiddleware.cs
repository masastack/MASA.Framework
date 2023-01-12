// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class AuthenticationMiddleware : ICallerMiddleware
{
    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        var tokenProvider = masaHttpContext.ServiceProvider.GetService<TokenProvider>();
        if (tokenProvider != null)
            masaHttpContext.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);

        var environment = masaHttpContext.ServiceProvider.GetService<IEnvironmentProvider>();
        if (environment != null)
            masaHttpContext.RequestMessage.Headers.Add(IsolationConsts.ENVIRONMENT, environment.GetEnvironment());

        return Task.CompletedTask;
    }
}
