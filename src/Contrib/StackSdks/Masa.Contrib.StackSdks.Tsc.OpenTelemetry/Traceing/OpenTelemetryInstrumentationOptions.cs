// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public class OpenTelemetryInstrumentationOptions
{
    public OpenTelemetryInstrumentationOptions(IServiceProvider serviceProvider)
    {
        Logger ??= serviceProvider.GetRequiredService<ILogger<OpenTelemetryInstrumentationOptions>>();
    }

    private readonly static AspNetCoreInstrumentationHandler aspNetCoreInstrumentationHandler = new();
    private readonly static HttpClientInstrumentHandler httpClientInstrumentHandler = new();
    internal static ILogger Logger { get; private set; }
    internal static long MaxBodySize { get; private set; } = 200 * 1 << 10;

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

    /// <summary>
    /// Build trace callback, allow to supplement the build process
    /// </summary>
    public Action<TracerProviderBuilder> BuildTraceCallback { get; set; }

    public static void SetMaxBodySize(string maxValue)
    {
        var regex = new Regex(@"\s+", RegexOptions.None, TimeSpan.FromSeconds(1));
        if (maxValue != null)
            maxValue = regex.Replace(maxValue, "");

        if (string.IsNullOrEmpty(maxValue))
            return;
        var unit = maxValue[^1];
        var isNum = int.TryParse(maxValue[..(maxValue.Length - 1)], out int num);
        if (!isNum || num <= 0) return;
        switch (unit)
        {
            case 'k':
            case 'K':
                MaxBodySize = num * 1 << 10;
                break;
            case 'm':
            case 'M':
                MaxBodySize = num * 1 << 20;
                break;
            default:
                MaxBodySize = num;
                break;
        }
    }

    public static void SetMaxBodySize(long maxByteValue)
    {
        if (maxByteValue > 0)
            MaxBodySize = maxByteValue;
    }
}
