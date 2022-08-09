// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Auth;

public class HttpEnvironmentDelegatingHandler : DelegatingHandler
{
    readonly IEnvironmentProvider _environmentProvider;
    readonly IHttpContextAccessor _httpContextAccessor;

    public HttpEnvironmentDelegatingHandler(IEnvironmentProvider environmentProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _environmentProvider = environmentProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestProvider = _httpContextAccessor.HttpContext?.RequestServices.GetService<IEnvironmentProvider>();
        if (requestProvider != null)
        {
            request.Headers.Add(IsolationConsts.ENVIRONMENT, requestProvider.GetEnvironment());
        }
        else
        {
            request.Headers.Add(IsolationConsts.ENVIRONMENT, _environmentProvider.GetEnvironment());
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
