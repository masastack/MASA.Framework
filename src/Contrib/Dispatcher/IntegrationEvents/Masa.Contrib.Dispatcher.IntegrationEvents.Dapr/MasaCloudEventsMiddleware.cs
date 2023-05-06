namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

internal class MasaCloudEventsMiddleware
{
    private const string CONTENT_TYPE = "application/masacloudevents+json";
    private readonly RequestDelegate _next;
    private readonly IsolationOptions _isolationOptions;

    public MasaCloudEventsMiddleware(RequestDelegate next, IOptions<IsolationOptions> isolationOptions)
    {
        this._next = next;
        this._isolationOptions = isolationOptions.Value;
    }

    public Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
    {
        if (!this.MatchesContentType(httpContext, out var charset))
        {
            return this._next(httpContext);
        }

        return this.ProcessBodyAsync(httpContext, serviceProvider, charset);
    }

    private async Task ProcessBodyAsync(HttpContext httpContext, IServiceProvider serviceProvider, string charSet)
    {
        JsonElement json;
        if (string.Equals(charSet, Encoding.UTF8.WebName, StringComparison.OrdinalIgnoreCase))
        {
            json = await JsonSerializer.DeserializeAsync<JsonElement>(httpContext.Request.Body);
        }
        else
        {
            using (var reader = new HttpRequestStreamReader(httpContext.Request.Body, Encoding.GetEncoding(charSet)))
            {
                var text = await reader.ReadToEndAsync();
                json = JsonSerializer.Deserialize<JsonElement>(text);
            }
        }

        Stream body;
        string? contentType;

        //
        var isDataSet = json.TryGetProperty("data", out var data);
        var isIsolationSet = json.TryGetProperty("isolation", out var isolation);

        if (isIsolationSet)
        {
            InitializeIsolation(serviceProvider, isolation.EnumerateObject());
        }

        if (isDataSet)
        {
            contentType = this.GetDataContentType(json, out var isJson);

            // If the value is anything other than a JSON string, treat it as JSON. Cloud Events requires
            // non-JSON text to be enclosed in a JSON string.
            isJson |= data.ValueKind != JsonValueKind.String;

            body = new MemoryStream();
            if (isJson)
            {
                // Rehydrate body from JSON payload
                await JsonSerializer.SerializeAsync(body, data);
            }
            else
            {
                // Rehydrate body from contents of the string
                var text = data.GetString();
                await using var writer = new HttpResponseStreamWriter(body, Encoding.UTF8);
                await writer.WriteAsync(text);
            }

            body.Seek(0L, SeekOrigin.Begin);
        }
        else
        {
            body = new MemoryStream();
            contentType = null;
        }

        var originalBody = httpContext.Request.Body;
        var originalContentType = httpContext.Request.ContentType;

        try
        {
            httpContext.Request.Body = body;
            httpContext.Request.ContentType = contentType;

            await this._next(httpContext);
        }
        finally
        {
            httpContext.Request.ContentType = originalContentType;
            httpContext.Request.Body = originalBody;
        }
    }

    void InitializeIsolation(IServiceProvider serviceProvider, JsonElement.ObjectEnumerator isolationEnumerator)
    {
        foreach (var item in isolationEnumerator)
        {
            if (item.Name.Equals(_isolationOptions.MultiEnvironmentName, StringComparison.OrdinalIgnoreCase))
            {
                var multiEnvironmentSetter = serviceProvider.GetService<IMultiEnvironmentSetter>();
                MasaArgumentException.ThrowIfNull(multiEnvironmentSetter);
                multiEnvironmentSetter.SetEnvironment(item.Value.ToString());
            }
            else if (item.Name.Equals(_isolationOptions.MultiTenantIdName, StringComparison.OrdinalIgnoreCase))
            {
                var multiTenantSetter = serviceProvider.GetService<IMultiTenantSetter>();
                MasaArgumentException.ThrowIfNull(multiTenantSetter);
                multiTenantSetter.SetTenant(new Tenant(item.Value.ToString()));
            }
        }
    }

    private string? GetDataContentType(JsonElement json, out bool isJson)
    {
        string? contentType;
        if (json.TryGetProperty("datacontenttype", out var dataContentType) &&
            dataContentType.ValueKind == JsonValueKind.String &&
            MediaTypeHeaderValue.TryParse(dataContentType.GetString(), out var parsed))
        {
            contentType = dataContentType.GetString();
            isJson =
                parsed.MediaType.Equals("application/json", StringComparison.Ordinal) ||
                parsed.Suffix.EndsWith("+json", StringComparison.Ordinal);

            // Since S.T.Json always outputs utf-8, we may need to normalize the data content type
            // to remove any charset information. We generally just assume utf-8 everywhere, so omitting
            // a charset is a safe bet.
            if (contentType != null && contentType.Contains("charset"))
            {
                parsed.Charset = StringSegment.Empty;
                contentType = parsed.ToString();
            }
        }
        else
        {
            // assume JSON is not specified.
            contentType = "application/json";
            isJson = true;
        }

        return contentType;
    }

    private bool MatchesContentType(HttpContext httpContext, [NotNullWhen(true)] out string? charset)
    {
        if (httpContext.Request.ContentType == null)
        {
            charset = null;
            return false;
        }

        // Handle cases where the content type includes additional parameters like charset.
        // Doing the string comparison up front so we can avoid allocation.
        if (!httpContext.Request.ContentType.StartsWith(CONTENT_TYPE))
        {
            charset = null;
            return false;
        }

        if (!MediaTypeHeaderValue.TryParse(httpContext.Request.ContentType, out var parsed))
        {
            charset = null;
            return false;
        }

        if (parsed.MediaType != CONTENT_TYPE)
        {
            charset = null;
            return false;
        }

        charset = parsed.Charset.Length > 0 ? parsed.Charset.Value : "UTF-8";
        return true;
    }
}
