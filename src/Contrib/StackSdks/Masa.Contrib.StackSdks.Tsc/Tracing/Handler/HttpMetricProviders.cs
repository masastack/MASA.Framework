// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Tracing.Handler;

public sealed class HttpMetricProviders
{
    private HttpMetricProviders() { }

    static HttpMetricProviders()
    {
        Meter = new Meter("masa stack metrics", "1.0.0");
        RegisterProvider(typeof(HttpRequestCountMeter), typeof(HttpRequestDurationMeter));
    }

    internal static Meter Meter { get; private set; }

    private readonly static List<IHttpRequestMetric> _httpRequestMetricProviders = new();

    private readonly static List<IHttpResponseMetric> _httpResponseMetricProviders = new();

    private readonly static List<IHttpRequestMessageMetric> _httpRequestMessageMetricProviders = new();

    private readonly static List<IHttpResponseMessageMetric> _httpResponseMessageMetricProviders = new();

    private readonly static List<IHttpWebRequestMetric> _httpWebRequestMetricProviders = new();

    private readonly static List<IHttpWebResponseMetric> _httpWebResponseMetricProviders = new();

    public static void RegisterProvider(params Type[] types)
    {
        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();
            var constructor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, new Type[] { typeof(Meter) });
            if (constructor == null)
                continue;
            object instance = constructor.Invoke(new object[] { Meter });

            //DiagnosticListener.AllListeners.Subscribe(new DiagnosticSourceListener())
            if (interfaces.Any(i => i == typeof(IHttpRequestMetric)))
            {
                Add(_httpRequestMetricProviders, instance, type);
            }
            if (interfaces.Any(i => i == typeof(IHttpResponseMetric)))
            {
                Add(_httpResponseMetricProviders, instance, type);
            }
            if (interfaces.Any(i => i == typeof(IHttpRequestMessageMetric)))
            {
                Add(_httpRequestMessageMetricProviders, instance, type);
            }
            if (interfaces.Any(i => i == typeof(IHttpResponseMessageMetric)))
            {
                Add(_httpResponseMessageMetricProviders, instance, type);
            }
            if (interfaces.Any(i => i == typeof(IHttpWebRequestMetric)))
            {
                Add(_httpWebRequestMetricProviders, instance, type);
            }
            if (interfaces.Any(i => i == typeof(IHttpWebResponseMetric)))
            {
                Add(_httpWebResponseMetricProviders, instance, type);
            }
        }
    }

    private static void Add<T>(List<T> list, object obj, Type type) where T : class
    {
        if (list.Any(t => t.GetType() == type))
            return;
        list.Add((T)obj);
    }

    internal static void AddHttpRequestMetric(HttpRequest httpRequest)
    {
        _httpRequestMetricProviders.ForEach(p => p.Handle(httpRequest));
    }

    internal static void AddHttpResponseMetric(HttpResponse httpResponse)
    {
        _httpResponseMetricProviders.ForEach(p => p.Handle(httpResponse));
    }

    internal static void AddHttpRequestMessageMetric(HttpRequestMessage httpRequestMessage)
    {
        _httpRequestMessageMetricProviders.ForEach(p => p.Handle(httpRequestMessage));
    }

    internal static void AddHttpResponseMessageMetric(HttpResponseMessage httpResponseMessage)
    {
        _httpResponseMessageMetricProviders.ForEach(p => p.Handle(httpResponseMessage));
    }

    internal static void AddHttpWebRequestMetric(HttpWebRequest httpWebRequest)
    {
        _httpWebRequestMetricProviders.ForEach(p => p.Handle(httpWebRequest));
    }

    internal static void AddHttpWebResponseMetric(HttpWebResponse httpWebResponse)
    {
        _httpWebResponseMetricProviders.ForEach(p => p.Handle(httpWebResponse));
    }
}
