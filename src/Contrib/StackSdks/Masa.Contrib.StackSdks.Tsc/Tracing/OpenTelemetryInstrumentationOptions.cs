// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class OpenTelemetryInstrumentationOptions
{
    private readonly static AspNetCoreInstrumentationHandler aspNetCoreInstrumentationHandler = new();
    private readonly static HttpClientInstrumentHandler httpClientInstrumentHandler = new();

    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<AspNetCoreInstrumentationOptions> AspNetCoreInstrumentationOptions { get; set; } = options =>
    {
        options.EnrichWithHttpRequest = aspNetCoreInstrumentationHandler.OnHttpRequest;
        options.EnrichWithHttpResponse = aspNetCoreInstrumentationHandler.OnHttpResponse;
        options.EnrichWithException = aspNetCoreInstrumentationHandler.OnException;
    };

    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<HttpClientInstrumentationOptions> HttpClientInstrumentationOptions { get; set; } = options =>
    {
        options.EnrichWithException = httpClientInstrumentHandler.OnException;
        options.EnrichWithHttpRequestMessage = httpClientInstrumentHandler.OnHttpRequestMessage;
        options.EnrichWithHttpResponseMessage = httpClientInstrumentHandler.OnHttpResponseMessage;
        options.EnrichWithHttpWebResponse = httpClientInstrumentHandler.OnHttpWebResponse;
        options.EnrichWithHttpWebRequest = httpClientInstrumentHandler.OnHttpWebRequest;
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
}
