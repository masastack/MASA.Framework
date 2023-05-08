// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

internal class MasaCloudEventsMiddleware
{
    private const string CONTENT_TYPE = "application/masacloudevents+json";
    private readonly RequestDelegate _next;
    private readonly IsolationOptions _isolationOptions;

    public MasaCloudEventsMiddleware(RequestDelegate next, IOptions<IsolationOptions> isolationOptions)
    {
        this._next = next;
        this._isolationOptions = isolationOptions.Value;
    }

    [ExcludeFromCodeCoverage]
    public Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
    {
        if (!MatchesContentType(httpContext, out var charset))
        {
            return this._next(httpContext);
        }

        return this.ProcessBodyAsync(httpContext, serviceProvider, charset);
    }

    public async Task ProcessBodyAsync(HttpContext httpContext, IServiceProvider serviceProvider, string charSet)
    {
        JsonElement json;
        if (string.Equals(charSet, Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase))
        {
            json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
        }
        else
        {
            using var reader = new HttpRequestStreamReader(httpContext.Request.Body, Encoding.GetEncoding(charSet));
            var text = await reader.ReadToEndAsync();
            json = JsonSerializer.Deserialize<JsonElement>(text);
        }

        Stream body;

        var isDataSet = json.TryGetProperty("data", out var data);
        var isIsolationSet = json.TryGetProperty("isolation", out var isolation);

        if (isIsolationSet)
        {
            InitializeIsolation(serviceProvider, isolation.EnumerateObject());
        }

        if (isDataSet)
        {
            body = new MemoryStream();

            await JsonSerializer.SerializeAsync(body, data);

            body.Seek(0L, SeekOrigin.Begin);
        }
        else
        {
            body = new MemoryStream();
        }

        var originalBody = httpContext.Request.Body;
        var originalContentType = httpContext.Request.ContentType;

        try
        {
            httpContext.Request.Body = body;

            await this._next(httpContext);
        }
        finally
        {
            httpContext.Request.ContentType = originalContentType;
            httpContext.Request.Body = originalBody;
        }
    }

    public void InitializeIsolation(IServiceProvider serviceProvider, JsonElement.ObjectEnumerator isolationEnumerator)
    {
        foreach (var item in isolationEnumerator)
        {
            if (item.Name.Equals(_isolationOptions.MultiEnvironmentName, StringComparison.OrdinalIgnoreCase))
            {
                var multiEnvironmentSetter = serviceProvider.GetService<IMultiEnvironmentSetter>();
                MasaArgumentException.ThrowIfNull(multiEnvironmentSetter);
                multiEnvironmentSetter.SetEnvironment(item.Value.ToString());
            }
            else if (item.Name.Equals(_isolationOptions.MultiTenantIdName, StringComparison.OrdinalIgnoreCase))
            {
                var multiTenantSetter = serviceProvider.GetService<IMultiTenantSetter>();
                MasaArgumentException.ThrowIfNull(multiTenantSetter);
                multiTenantSetter.SetTenant(new Tenant(item.Value.ToString()));
            }
        }
    }

    public static bool MatchesContentType(HttpContext httpContext, [NotNullWhen(true)] out string? charset)
    {
        if (httpContext.Request.ContentType == null)
        {
            charset = null;
            return false;
        }

        // Handle cases where the content type includes additional parameters like charset.
        // Doing the string comparison up front so we can avoid allocation.
        if (!httpContext.Request.ContentType.StartsWith(CONTENT_TYPE))
        {
            charset = null;
            return false;
        }

        if (!MediaTypeHeaderValue.TryParse(httpContext.Request.ContentType, out var parsed))
        {
            charset = null;
            return false;
        }

        if (parsed.MediaType != CONTENT_TYPE)
        {
            charset = null;
            return false;
        }

        charset = parsed.Charset.Length > 0 ? parsed.Charset.Value : "UTF-8";
        return true;
    }
}
