// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Metrics.Instrumentation.Http;

public interface IHttpRequestMetric
{
    void Handle(HttpRequest httpRequest);
}

public interface IHttpResponseMetric
{
    void Handle(HttpResponse httpResponse);
}

public interface IHttpRequestMessageMetric
{
    void Handle(HttpRequestMessage httpRequestMessage);
}

public interface IHttpResponseMessageMetric
{
    void Handle(HttpResponseMessage httpResponseMessage);
}

public interface IHttpWebRequestMetric
{
    void Handle(HttpWebRequest httpWebRequest);
}

public interface IHttpWebResponseMetric
{
    void Handle(HttpWebResponse httpWebResponse);
}

public interface ICustomMetrics
{
    void Handle(string name, object payload);
}
