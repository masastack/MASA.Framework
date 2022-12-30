// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Metrics.Instrumentation.Http;

public class HttpRequestCountMeter : IHttpResponseMetric, IHttpRequestMetric
{
    private readonly Dictionary<string, Counter<long>> _dicCounts = new();
    private readonly Meter _meter;

    public HttpRequestCountMeter(Meter meter)
    {
        _meter = meter;
    }

    private Counter<long> CreateRequestCounter() => _meter.CreateCounter<long>("masa.http.request.count", "", "http request in count per seconds");

    private Counter<long> CreateReponseCounter() => _meter.CreateCounter<long>("masa.http.response.count", "", "http response in count per seconds");

    private IEnumerable<KeyValuePair<string, object?>> GetTags(HttpRequest httpRequest)
    {
        var list = new List<KeyValuePair<string, object?>>
        {
            new KeyValuePair<string, object?>("http.target", httpRequest.Path),
            new KeyValuePair<string, object?>("http.method", httpRequest.Method)
        };
        return list;
    }

    private IEnumerable<KeyValuePair<string, object?>> GetTags(HttpResponse httpResponse)
    {
        var list = new List<KeyValuePair<string, object?>>
        {
            new KeyValuePair<string, object?>("http.status_code", httpResponse.StatusCode),
            new KeyValuePair<string, object?>("http.method", httpResponse.HttpContext.Request.Method),
            new KeyValuePair<string, object?>("http.target", httpResponse.HttpContext.Request.Path)
        };
        return list;
    }

    public void Handle(HttpResponse httpResponse)
    {
        var httpRequest = httpResponse.HttpContext.Request;
        var path = httpRequest.Path;
        if (string.IsNullOrEmpty(path))
            return;

        Counter<long> counter;
        if (_dicCounts.ContainsKey(path))
        {
            counter = _dicCounts[path];
        }
        else
        {
            counter = CreateReponseCounter();
            _dicCounts.Add(path, counter);
        }
        var tags = GetTags(httpResponse);
        counter.Add(1, tags.ToArray());
    }

    public void Handle(HttpRequest httpRequest)
    {
        var path = httpRequest.Path;
        if (string.IsNullOrEmpty(path))
            return;

        Counter<long> counter;
        if (_dicCounts.ContainsKey(path))
        {
            counter = _dicCounts[path];
        }
        else
        {
            counter = CreateRequestCounter();
            _dicCounts.Add(path, counter);
        }
        var tags = GetTags(httpRequest);
        counter.Add(1, tags.ToArray());
    }
}
