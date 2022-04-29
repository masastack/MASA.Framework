namespace System.Diagnostics;

public static class ActivityExtension
{
    public static Activity AddMasaSupplement(this Activity activity, HttpRequest httpRequest)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Http.Flavor, httpRequest.Protocol);
        activity.SetTag(OpenTelemetryAttributeName.Http.Scheme, httpRequest.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Http.ClientIP, httpRequest.HttpContext?.Connection.RemoteIpAddress);
        activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentLength, httpRequest.ContentLength);
        activity.SetTag(OpenTelemetryAttributeName.Http.RequestContentType, httpRequest.ContentType);
        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        return activity;
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
            activity.AddTag("enduser.nick_name", httpResponse.HttpContext.User.FindFirst("https://masastack.com/security/authentication/MasaNickName")?.Value ?? string.Empty);
        }

        return activity;
    }

    public static async Task<Activity> AddMasaSupplement(this Activity activity, HttpRequestMessage httpRequest)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Http.Scheme, httpRequest.RequestUri.Scheme);
        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        if (httpRequest.Content is not null)
            activity.SetTag("http.request_content", await httpRequest.Content.ReadAsStringAsync());

        return activity;
    }

    public static Activity AddMasaSupplement(this Activity activity, HttpResponseMessage httpResponse)
    {
        if (activity is null) return activity;

        activity.SetTag(OpenTelemetryAttributeName.Host.Name, Dns.GetHostName());

        return activity;
    }
}
