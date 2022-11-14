// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class OpenTelemetryInstrumentationOptions
{
    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<AspNetCoreInstrumentationOptions> AspNetCoreInstrumentationOptions { get; set; } = options =>
    {
        options.Enrich = async (activity, eventName, rawObject) => await SetHttpTags(activity, eventName, rawObject);
    };

    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<HttpClientInstrumentationOptions> HttpClientInstrumentationOptions { get; set; } = options =>
    {
        options.Enrich = async (activity, eventName, rawObject) => await SetHttpTags(activity, eventName, rawObject);
    };

    /// <summary>
    /// Default record db statement for text. You can replace it or set null
    /// </summary>
    public Action<EntityFrameworkInstrumentationOptions> EntityFrameworkInstrumentationOptions { get; set; } = options =>
    {
        options.SetDbStatementForText = true;
    };

    public Action<ElasticsearchClientInstrumentationOptions> ElasticsearchClientInstrumentationOptions { get; set; } = options =>
     {
         options.ParseAndFormatRequest = true;
     };

    public Action<StackExchangeRedisCallsInstrumentationOptions> StackExchangeRedisCallsInstrumentationOptions { get; set; }

    public IConnectionMultiplexer Connection { get; set; }

    /// <summary>
    /// Build trace callback, allow to supplement the build process
    /// </summary>
    public Action<TracerProviderBuilder> BuildTraceCallback { get; set; }

    private static async Task SetHttpTags(Activity activity, string eventName, object rawObject)
    {
        if (eventName.Equals("OnStartActivity"))
        {
            if (rawObject is HttpRequest httpRequest)
            {
                await activity.AddMasaSupplement(httpRequest);
            }
            else if (rawObject is HttpRequestMessage httpRequestMessage)
            {
                await activity.AddMasaSupplement(httpRequestMessage);
            }
        }
        else if (eventName.Equals("OnStopActivity"))
        {
            if (rawObject is HttpResponse httpResponse)
            {
                activity.AddMasaSupplement(httpResponse);
            }
            else if (rawObject is HttpResponseMessage httpResponseMessage)
            {
                activity.AddMasaSupplement(httpResponseMessage);
            }
        }
        else if (eventName.Equals("OnException"))
        {
            SetSexceptionTags(activity, rawObject);
        }
    }

    private static void SetSexceptionTags(Activity activity, object rawObject)
    {
        if (rawObject is Exception exception)
        {
            activity.SetTag(OpenTelemetryAttributeName.Exception.MESSAGE, exception.Message);
            activity.SetTag(OpenTelemetryAttributeName.Exception.STACKTRACE, exception.ToString());
        }
    }
}
