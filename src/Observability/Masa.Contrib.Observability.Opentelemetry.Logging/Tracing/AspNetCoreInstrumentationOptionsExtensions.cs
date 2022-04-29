namespace Microsoft.Extensions.DependencyInjection;

public static class AspNetCoreInstrumentationOptionsExtensions
{
    private static List<string> _lstDefaultFilterIgnorePrefix = new List<string>()
    {
        "/swagger",
        "/healthz",
    };

    private static List<string> _lstBlazorFilterIgnorePrefix = new List<string>()
    {
        "/swagger",
        "/healthz",
        "/_blazor",
        "/_content",
    };

    private static List<string> _lstBlazorFilterIgnoreSuffix = new List<string>()
    {
        ".js",
        ".css",
        "/negotiate",
        ".ico",
        ".png",
        ".woff",
    };

    /// <summary>
    /// The default filter ignore list includes swagger.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="openTelemetryInstrumentationOptions"></param>
    public static void AppendDefaultFilter(this Action<AspNetCoreInstrumentationOptions> options, OpenTelemetryInstrumentationOptions openTelemetryInstrumentationOptions)
    {
        options += opt =>
        {
            opt.Filter = httpContext =>
            {
                return !_lstDefaultFilterIgnorePrefix.Any(prefix => httpContext.Request.Path.Value.StartsWith(prefix));
            };
        };

        openTelemetryInstrumentationOptions.AspNetCoreInstrumentationOptions = options;
    }

    /// <summary>
    /// The filter ignore list includes swagger and blazor and static files.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="openTelemetryInstrumentationOptions"></param>
    public static void AppendBlazorFilter(this Action<AspNetCoreInstrumentationOptions> options, OpenTelemetryInstrumentationOptions openTelemetryInstrumentationOptions)
    {
        options += opt =>
        {
            opt.Filter = httpContext =>
            {
                if (_lstBlazorFilterIgnorePrefix.Any(prefix => httpContext.Request.Path.Value.StartsWith(prefix)))
                    return false;

                if (_lstBlazorFilterIgnoreSuffix.Any(suffix => httpContext.Request.Path.Value.EndsWith(suffix)))
                    return false;

                return true;
            };
        };

        openTelemetryInstrumentationOptions.AspNetCoreInstrumentationOptions = options;
    }
}
