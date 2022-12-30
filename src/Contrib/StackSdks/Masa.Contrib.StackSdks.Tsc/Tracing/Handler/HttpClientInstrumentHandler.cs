// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Tracing.Handler;

public class HttpClientInstrumentHandler : ExceptionHandler
{
    public virtual bool IsSkipHttpRequestMessage(HttpRequestMessage httpRequestMessage)
    {
        return false;
    }

    public virtual bool IsSkipHttpWebRequest(HttpWebRequest httpWebRequest)
    {
        return false;
    }

    public virtual async void OnHttpRequestMessage(Activity activity, HttpRequestMessage httpRequestMessage)
    {
        await activity.AddMasaSupplement(httpRequestMessage);
        HttpMetricProviders.AddHttpRequestMessageMetric(httpRequestMessage);
    }

    public virtual void OnHttpResponseMessage(Activity activity, HttpResponseMessage httpResponseMessage)
    {
        activity.AddMasaSupplement(httpResponseMessage);
        HttpMetricProviders.AddHttpResponseMessageMetric(httpResponseMessage);
    }

    public virtual void OnHttpWebRequest(Activity activity, HttpWebRequest httpWebRequest)
    {

    }

    public virtual void OnHttpWebResponse(Activity activity, HttpWebResponse httpWebResponse)
    {

    }
}
