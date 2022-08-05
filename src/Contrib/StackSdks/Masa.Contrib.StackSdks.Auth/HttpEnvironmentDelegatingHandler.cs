// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Authorization;

namespace Masa.Contrib.StackSdks.Auth;

public class HttpEnvironmentDelegatingHandler : DelegatingHandler
{
    readonly AuthenticationStateProvider _authenticationStateProvider;
    readonly IEnvironmentProvider _environmentProvider;

    public HttpEnvironmentDelegatingHandler(AuthenticationStateProvider authenticationStateProvider, IEnvironmentProvider environmentProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _environmentProvider = environmentProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var envClaim = (await _authenticationStateProvider.GetAuthenticationStateAsync())
            .User.Claims.FirstOrDefault(c => c.Type == "environment");
        if (envClaim != null)
        {
            request.Headers.Add(ENVIRONMENT_KEY, _environmentProvider.GetEnvironment());
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
