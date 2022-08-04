// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public abstract class DefaultRequestMessage
{
    protected readonly IServiceProvider ServiceProvider;
    private readonly string _requestIdKey;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    protected readonly CallerFactoryOptions? Options;

    public DefaultRequestMessage(IServiceProvider serviceProvider,
        IOptions<CallerFactoryOptions>? options = null)
    {
        ServiceProvider = serviceProvider;
        _httpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
        Options = options?.Value;
        _requestIdKey = Options?.RequestIdKey ?? "Masa-Request-Id";
    }

    protected virtual void TrySetRequestId(HttpRequestMessage requestMessage)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext == null)
            return;

        if (!httpContext.Request.Headers.TryGetValue(_requestIdKey, out var requestId))
            requestId = Guid.NewGuid().ToString();

        if (requestMessage.Headers.All(h => h.Key != _requestIdKey))
            requestMessage.Headers.Add(_requestIdKey, requestId.ToString());
    }
}
