// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Metric.Instrumentation.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tracing.Handler
{
    public sealed class HttpMetricProviders
    {
        private HttpMetricProviders() { }

        static HttpMetricProviders()
        {
            Meter = new Meter("masa stack metrics", "1.0.0");
            RegisterProvider(typeof(HttpResponseMeter));
        }

        internal static Meter Meter { get; private set; }

        private readonly static List<IHttpRequestMetric> _httpRequestMetricProviders = new List<IHttpRequestMetric>();

        private readonly static List<IHttpResponseMetric> _httpResponseMetricProviders = new List<IHttpResponseMetric>();

        private readonly static List<IHttpRequestMessageMetric> _httpRequestMessageMetricProviders = new List<IHttpRequestMessageMetric>();

        private readonly static List<IHttpResponseMessageMetric> _httpResponseMessageMetricProviders = new List<IHttpResponseMessageMetric>();

        private readonly static List<IHttpWebRequestMetric> _httpWebRequestMetricProviders = new List<IHttpWebRequestMetric>();

        private readonly static List<IHttpWebResponseMetric> _httpWebResponseMetricProviders = new List<IHttpWebResponseMetric>();

        public static void RegisterProvider(params Type[] types)
        {
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                    Type.DefaultBinder,
                    new Type[] { typeof(Meter) },
                    new ParameterModifier[] { new ParameterModifier(1) });
                if (constructor == null)
                    continue;
                object instance = constructor.Invoke(new object[] { Meter });

                if (Array.Exists(interfaces, i => i == typeof(IHttpRequestMetric)))
                {
                    Add(_httpRequestMetricProviders, instance, type);
                }
                if (Array.Exists(interfaces, i => i == typeof(IHttpResponseMetric)))
                {
                    Add(_httpResponseMetricProviders, instance, type);
                }
                if (Array.Exists(interfaces, i => i == typeof(IHttpRequestMessageMetric)))
                {
                    Add(_httpRequestMessageMetricProviders, instance, type);
                }
                if (Array.Exists(interfaces, i => i == typeof(IHttpResponseMessageMetric)))
                {
                    Add(_httpResponseMessageMetricProviders, instance, type);
                }
                if (Array.Exists(interfaces, i => i == typeof(IHttpWebRequestMetric)))
                {
                    Add(_httpWebRequestMetricProviders, instance, type);
                }
                if (Array.Exists(interfaces, i => i == typeof(IHttpWebResponseMetric)))
                {
                    Add(_httpWebResponseMetricProviders, instance, type);
                }
            }
        }

        private static void Add<T>(List<T> list, object obj, Type type) where T : class
        {
            if (list.Exists(t => t.GetType() == type))
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
}
