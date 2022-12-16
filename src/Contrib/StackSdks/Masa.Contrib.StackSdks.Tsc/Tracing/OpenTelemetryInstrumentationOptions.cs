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
        options.EnrichWithHttpRequest = async (activity, httpRequest) => await activity.AddMasaSupplement(httpRequest);
        options.EnrichWithHttpResponse = (activity, httpResponse) => activity.AddMasaSupplement(httpResponse);
        options.EnrichWithException = SetSexceptionTags;
    };

    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<HttpClientInstrumentationOptions> HttpClientInstrumentationOptions { get; set; } = options =>
    {
        options.EnrichWithHttpRequestMessage = async (activity, httpRequestMessage) => await activity.AddMasaSupplement(httpRequestMessage);
        options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) => activity.AddMasaSupplement(httpResponseMessage);
        options.EnrichWithException= SetSexceptionTags;
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

    private static void SetSexceptionTags(Activity activity, Exception exception)
    {
        if (exception != null)
        {
            activity.SetTag(OpenTelemetryAttributeName.Exception.MESSAGE, exception.Message);
            activity.SetTag(OpenTelemetryAttributeName.Exception.STACKTRACE, exception.ToString());
        }
    }
}
