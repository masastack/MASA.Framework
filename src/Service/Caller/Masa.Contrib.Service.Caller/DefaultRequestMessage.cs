// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public abstract class DefaultRequestMessage
{
    private readonly string _requestIdKey;
    private readonly IRequestIdGenerator _requestIdGenerator;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    protected readonly CallerFactoryOptions Options;

    public DefaultRequestMessage(IOptionsFactory<CallerFactoryOptions> optionsFactory,
        IRequestIdGenerator requestIdGenerator,
        IHttpContextAccessor? httpContextAccessor = null)
    {
        _requestIdGenerator = requestIdGenerator;
        _httpContextAccessor = httpContextAccessor;
        Options = optionsFactory.Create(Microsoft.Extensions.Options.Options.DefaultName);
        _requestIdKey = Options.RequestIdKey ?? "Masa-Request-Id";
    }

    protected void TrySetRequestId(HttpRequestMessage requestMessage)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext == null)
            return;

        if (!httpContext.Request.Headers.TryGetValue(_requestIdKey, out var requestId))
            requestId = _requestIdGenerator.NewId();

        if (requestMessage.Headers.All(h => h.Key != _requestIdKey))
            requestMessage.Headers.Add(_requestIdKey, requestId.ToString());
    }
}
