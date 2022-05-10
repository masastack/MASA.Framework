// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.Diagnostics;

public static class ActivityExtension
{
    public static async Task<Activity> AddMasaSupplement(this Activity activity, HttpRequest httpRequest)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Http.Flavor, httpRequest.Protocol);
        activity.SetTag(OpenTelemetryAttributeName.Http.Scheme, httpRequest.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Http.ClientIP, httpRequest.HttpContext?.Connection.RemoteIpAddress);
        activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentLength, httpRequest.ContentLength);
        activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentType, httpRequest.ContentType);
        if (httpRequest.Body != null)
        {
            if (!httpRequest.Body.CanSeek)
                httpRequest.EnableBuffering();
            activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentBody, await httpRequest.HttpContext.Request.Body.ReadAsStringAsync(GetHttpRequestEncoding(httpRequest)));
        }
        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        return activity;
    }

    private static Encoding GetHttpRequestEncoding(HttpRequest httpRequest)
    {
        if (httpRequest.Body != null)
        {
            var contentType = httpRequest.HttpContext.Request.ContentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                var attr = MediaTypeHeaderValue.Parse(contentType);
                if (attr != null && !string.IsNullOrEmpty(attr.CharSet))
                    return Encoding.GetEncoding(attr.CharSet);
            }
        }

        return null;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpResponse httpResponse)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Http.ResponseContentLength, httpResponse.ContentLength);
        activity.SetTag(OpenTelemetryAttributeName.Http.ResponseContentType, httpResponse.ContentType);
        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        if ((httpResponse.HttpContext.User?.Claims.Count() ?? 0) > 0)
        {
            activity.AddTag(OpenTelemetryAttributeName.EndUser.Id, httpResponse.HttpContext.User.FindFirst("sub")?.Value ?? string.Empty);
            activity.AddTag(OpenTelemetryAttributeName.EndUser.UserName, httpResponse.HttpContext.User.FindFirst("https://masastack.com/security/authentication/MasaNickName")?.Value ?? string.Empty);
        }

        return activity;
    }

    public static async Task<Activity> AddMasaSupplement(this Activity activity, HttpRequestMessage httpRequest)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Http.Scheme, httpRequest.RequestUri.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        if (httpRequest.Content is not null)
        {
            var st = await httpRequest.Content.ReadAsStreamAsync();
            activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentBody, await st.ReadAsStringAsync(GetHttpRequestMessageEncoding(httpRequest)));
        }

        return activity;
    }

    private static Encoding GetHttpRequestMessageEncoding(HttpRequestMessage httpRequest)
    {
        if (httpRequest.Content is not null)
        {
            var encodeStr = httpRequest.Content.Headers?.ContentType?.CharSet;

            if (!string.IsNullOrEmpty(encodeStr))
            {
                return Encoding.GetEncoding(encodeStr);
            }
        }

        return null;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpResponseMessage httpResponse)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        return activity;
    }

}
