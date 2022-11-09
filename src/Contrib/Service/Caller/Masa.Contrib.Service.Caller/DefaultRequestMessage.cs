// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Globalization;

namespace Masa.Contrib.Service.Caller;

public abstract class DefaultRequestMessage
{
    protected readonly IServiceProvider ServiceProvider;
    private readonly string _requestIdKey;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    protected readonly CallerFactoryOptions? Options;

    protected DefaultRequestMessage(IServiceProvider serviceProvider,
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

    protected virtual void TrySetCulture(HttpRequestMessage requestMessage)
    {
        var cultures = new List<(string Key, string Value)>
        {
            ("c", CultureInfo.CurrentCulture.Name),
            ("uic", CultureInfo.CurrentUICulture.Name)
        };
        TrySetCulture(requestMessage, cultures);
    }

    protected virtual void TrySetCulture(HttpRequestMessage requestMessage, List<(string Key, string Value)> cultures)
    {
        var name = "cookie";
        if (requestMessage.Headers.TryGetValues(name, out IEnumerable<string>? cookieValues))
            requestMessage.Headers.Remove(name);
        string value = System.Web.HttpUtility.UrlEncode(string.Join('|', cultures.Select(c => $"{c.Key}={c.Value}")));
        var cookies = cookieValues?.ToList() ?? new List<string>();
        cookies.Add($".AspNetCore.Culture={value}");
        requestMessage.Headers.Add(name, cookies);
    }
}
