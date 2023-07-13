// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AspNetCoreInstrumentationOptionsExtensions
{
    private static readonly List<string> _CommonFilterIgnoreSuffix = new List<string>()
{
    ".js",
    ".css",
    ".ico",
    ".png",
    ".woff",
    ".icon"
};

    private static readonly List<string> _CommonFilterIgnorePrefix = new List<string>()
{
    "/swagger",
    "/healthz"
};

    private static readonly List<string> _BlazorFilterIgnorePrefix = new List<string>()
{
    "/_blazor",
    "/_content",
    "/negotiate"
};

    private static bool IsInterruptSignalrTracing = true;

    /// <summary>
    /// The default filter ignore list includes swagger.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="openTelemetryInstrumentationOptions"></param>
    /// <param name="isInterruptSignalrTracing"></param>
    public static void AppendDefaultFilter(this Action<AspNetCoreInstrumentationOptions> options,
        OpenTelemetryInstrumentationOptions openTelemetryInstrumentationOptions,
        bool isInterruptSignalrTracing)
    {
        IsInterruptSignalrTracing = isInterruptSignalrTracing;
        options += opt =>
        {
            opt.Filter = IsDefaultFilter;
        };
        openTelemetryInstrumentationOptions.AspNetCoreInstrumentationOptions += options;
    }

    private static bool IsDefaultFilter(HttpContext httpContext) => !(IsInterruptSignalrTracing && IsWebsocket(httpContext)
                 || IsReuqestPathMatchPrefix(httpContext, _CommonFilterIgnorePrefix)
                 || IsReuqestPathMatchSuffix(httpContext, _CommonFilterIgnoreSuffix));

    /// <summary>
    /// The filter ignore list includes swagger and blazor and static files.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="openTelemetryInstrumentationOptions"></param>
    /// <param name="isInterruptSignalrTracing"></param>
    public static void AppendBlazorFilter(this Action<AspNetCoreInstrumentationOptions> options,
        OpenTelemetryInstrumentationOptions openTelemetryInstrumentationOptions,
        bool isInterruptSignalrTracing)
    {
        IsInterruptSignalrTracing = isInterruptSignalrTracing;
        options += opt =>
        {
            opt.Filter = httpContext => IsDefaultFilter(httpContext) && IsBlazorFilter(httpContext);
        };
        openTelemetryInstrumentationOptions.AspNetCoreInstrumentationOptions += options;
    }

    private static bool IsBlazorFilter(HttpContext httpContext) => !IsReuqestPathMatchPrefix(httpContext, _BlazorFilterIgnorePrefix);

    private static bool IsWebsocket(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.ContainsKey("Connection")
            && httpContext.Request.Headers.ContainsKey(httpContext.Request.Headers["Connection"]))
        {                
            Activity.Current = null;
            return true;
        }
        return false;
    }

    private static bool IsReuqestPathMatchSuffix(HttpContext httpContext, List<string> suffix)
    {
        return !string.IsNullOrEmpty(httpContext.Request.Path.Value) && suffix.Exists(str => httpContext.Request.Path.Value.EndsWith(str));
    }

    private static bool IsReuqestPathMatchPrefix(HttpContext httpContext, List<string> prefix)
    {
        return !string.IsNullOrEmpty(httpContext.Request.Path.Value) && prefix.Exists(str => httpContext.Request.Path.Value.StartsWith(str));
    }
}
