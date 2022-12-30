// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Metrics.Instrumentation.Http;

public class HttpRequestDurationMeter : IHttpResponseMetric
{
    private readonly Dictionary<string, Histogram<long>> _dicCounts = new();
    private readonly Meter _meter;

    public HttpRequestDurationMeter(Meter meter)
    {
        _meter = meter;
    }

    private Histogram<long> CreateCounter() => _meter.CreateHistogram<long>("masa.http.request.duration", "ms", "http request duration");

    private IEnumerable<KeyValuePair<string, object?>> GetTags(HttpResponse httpResponse)
    {
        var list = new List<KeyValuePair<string, object?>>
        {
            new KeyValuePair<string, object?>("http.url", httpResponse.HttpContext.Request.Path),
            new KeyValuePair<string, object?>("http.status_code", httpResponse.StatusCode),
            new KeyValuePair<string, object?>("http.method", httpResponse.HttpContext.Request.Method)
        };
        return list;
    }

    public void Handle(HttpResponse httpResponse)
    {
        var activity = Activity.Current!;
        var httpRequest = httpResponse.HttpContext.Request;
        var path = httpRequest.Path;
        if (string.IsNullOrEmpty(path))
            return;

        Histogram<long> counter;
        if (_dicCounts.ContainsKey(path))
        {
            counter = _dicCounts[path];
        }
        else
        {
            counter = CreateCounter();
            _dicCounts.Add(path, counter);
        }
        var tags = GetTags(httpResponse);
        counter.Record((long)Math.Floor(activity.Duration.TotalMilliseconds), tags.ToArray());
    }
}
