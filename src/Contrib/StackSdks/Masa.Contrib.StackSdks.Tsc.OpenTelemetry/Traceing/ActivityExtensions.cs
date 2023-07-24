// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.Diagnostics;

public static class ActivityExtension
{
    public static Activity AddMasaSupplement(this Activity activity, HttpRequest httpRequest)
    {
        activity.SetTag(OpenTelemetryAttributeName.Http.FLAVOR, httpRequest.Protocol);
        activity.SetTag(OpenTelemetryAttributeName.Http.SCHEME, httpRequest.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Http.CLIENT_IP, httpRequest.HttpContext?.Connection?.RemoteIpAddress);
        activity.SetTag(OpenTelemetryAttributeName.Http.REQUEST_CONTENT_LENGTH, httpRequest.ContentLength);
        activity.SetTag(OpenTelemetryAttributeName.Http.REQUEST_CONTENT_TYPE, httpRequest.ContentType);
        if (httpRequest.Body != null)
        {
            if (!httpRequest.Body.CanSeek)
                httpRequest.EnableBuffering();
            SetActivityBody(activity, httpRequest.Body, GetHttpRequestEncoding(httpRequest)).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        activity.SetTag(OpenTelemetryAttributeName.Host.NAME, Dns.GetHostName());
        return activity;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpRequestMessage httpRequest)
    {
        activity.SetTag(OpenTelemetryAttributeName.Http.SCHEME, httpRequest.RequestUri?.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Host.NAME, Dns.GetHostName());

        if (httpRequest.Content != null)
        {
            SetActivityBody(activity,
                httpRequest.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult(),
                GetHttpRequestMessageEncoding(httpRequest)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        return activity;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpResponse httpResponse)
    {
        activity.SetTag(OpenTelemetryAttributeName.Http.RESPONSE_CONTENT_LENGTH, httpResponse.ContentLength);
        activity.SetTag(OpenTelemetryAttributeName.Http.RESPONSE_CONTENT_TYPE, httpResponse.ContentType);
        activity.SetTag(OpenTelemetryAttributeName.Host.NAME, Dns.GetHostName());

        if ((httpResponse.HttpContext.User?.Claims.Count() ?? 0) > 0)
        {
            activity.AddTag(OpenTelemetryAttributeName.EndUser.ID, httpResponse.HttpContext.User?.FindFirst("sub")?.Value ?? string.Empty);
            activity.AddTag(OpenTelemetryAttributeName.EndUser.USER_NICK_NAME, httpResponse.HttpContext.User?.FindFirst("https://masastack.com/security/authentication/MasaNickName")?.Value ?? string.Empty);
        }

        return activity;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpResponseMessage httpResponse)
    {
        activity.SetTag(OpenTelemetryAttributeName.Host.NAME, Dns.GetHostName());
        return activity;
    }

    private static Encoding? GetHttpRequestEncoding(HttpRequest httpRequest)
    {
        if (httpRequest.Body != null)
        {
            var contentType = httpRequest.ContentType;
            if (!string.IsNullOrEmpty(contentType)
                && MediaTypeHeaderValue.TryParse(contentType, out var attr)
                && attr != null
                && !string.IsNullOrEmpty(attr.CharSet))
                return Encoding.GetEncoding(attr.CharSet);
        }

        return null;
    }

    private static Encoding? GetHttpRequestMessageEncoding(HttpRequestMessage httpRequest)
    {
        if (httpRequest.Content != null)
        {
            var encodeStr = httpRequest.Content.Headers?.ContentType?.CharSet;

            if (!string.IsNullOrEmpty(encodeStr))
            {
                return Encoding.GetEncoding(encodeStr);
            }
        }

        return null;
    }

    private static async Task SetActivityBody(Activity activity, Stream inputSteam, Encoding? encoding = null)
    {
        (long length, string? body) = await inputSteam.ReadAsStringAsync(encoding);

        if (length <= 0)
            return;
        if (length - OpenTelemetryInstrumentationOptions.MaxBodySize > 0)
        {
            OpenTelemetryInstrumentationOptions.Logger?.LogInformation("Request body in base64 encode: {Body}", body);
        }
        else
        {
            activity.SetTag(OpenTelemetryAttributeName.Http.REQUEST_CONTENT_BODY, body);
        }
    }
}
