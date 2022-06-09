// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth;

public class HttpEnvironmentDelegatingHandler : DelegatingHandler
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IEnvironmentProvider _environmentProvider;

    public HttpEnvironmentDelegatingHandler(IHttpContextAccessor httpContextAccessor, IEnvironmentProvider environmentProvider)
    {
        _httpContextAccessor = httpContextAccessor;
        _environmentProvider = environmentProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var envClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "env");
        if (envClaim != null)
        {
            _httpContextAccessor.HttpContext?.Items.TryAdd(ENVIRONMENT_KEY, _environmentProvider.GetEnvironment());
            request.Headers.Add(ENVIRONMENT_KEY, _environmentProvider.GetEnvironment());
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
