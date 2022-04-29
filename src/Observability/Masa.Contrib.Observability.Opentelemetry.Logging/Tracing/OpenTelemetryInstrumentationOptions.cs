namespace Microsoft.Extensions.DependencyInjection;

public class OpenTelemetryInstrumentationOptions
{
    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<AspNetCoreInstrumentationOptions> AspNetCoreInstrumentationOptions { get; set; } = options =>
    {
        options.Enrich = async (activity, eventName, rawObject) =>
        {
            if (eventName.Equals("OnStartActivity"))
            {
                if (rawObject is HttpRequest httpRequest)
                {
                    activity.AddMasaSupplement(httpRequest);
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
            }
            else if (eventName.Equals("OnException"))
            {
                if (rawObject is Exception exception)
                {
                    activity.SetTag("stackTrace", exception.StackTrace);
                }
            }
        };
    };

    /// <summary>
    /// Default record all data. You can replace it or set null
    /// </summary>
    public Action<HttpClientInstrumentationOptions> HttpClientInstrumentationOptions { get; set; } = options =>
    {
        options.Enrich = (activity, eventName, rawObject) =>
        {
            if (eventName.Equals("OnStartActivity"))
            {
                if (rawObject is HttpRequestMessage httpRequest)
                {
                    activity.AddMasaSupplement(httpRequest);
                }
            }
            else if (eventName.Equals("OnStopActivity"))
            {
                if (rawObject is HttpResponseMessage httpResponse)
                {
                    activity.AddMasaSupplement(httpResponse);
                }
            }
            else if (eventName.Equals("OnException"))
            {
                if (rawObject is Exception exception)
                {
                    activity.SetTag("stackTrace", exception.StackTrace);
                }
            }
        };
    };

    /// <summary>
    /// Default record db statement for text. You can replace it or set null
    /// </summary>
    public Action<EntityFrameworkInstrumentationOptions> EntityFrameworkInstrumentationOptions { get; set; } = options =>
    {
        options.SetDbStatementForText = true;
    };

    public Action<ElasticsearchClientInstrumentationOptions> ElasticsearchClientInstrumentationOptions { get; set; }= options=>
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
