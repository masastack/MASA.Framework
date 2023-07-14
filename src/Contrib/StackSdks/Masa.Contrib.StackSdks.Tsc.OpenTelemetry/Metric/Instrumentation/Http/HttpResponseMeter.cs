// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Metric.Instrumentation.Http;

public class HttpResponseMeter : IHttpResponseMetric
{
    private readonly Histogram<double> responseHistogram;

    public HttpResponseMeter(Meter meter)
    {
        responseHistogram = meter.CreateHistogram<double>("http.response", "ms", "http response duration");
    }

    public void Handle(HttpResponse httpResponse)
    {
        var httpRequest = httpResponse.HttpContext.Request;
        var path = httpRequest.Path;
        if (string.IsNullOrEmpty(path))
            return;

        var tags = new KeyValuePair<string, object?>[]
        {
            new KeyValuePair<string, object?>("http.status_code", httpResponse.StatusCode),
            new KeyValuePair<string, object?>("http.method", httpResponse.HttpContext.Request.Method),
            new KeyValuePair<string, object?>("http.scheme", httpResponse.HttpContext.Request.Scheme),
            new KeyValuePair<string, object?>("http.target", httpResponse.HttpContext.Request.Path)
        };
        responseHistogram.Record(Activity.Current!.Duration.TotalMilliseconds, tags.ToArray());
    }
}
