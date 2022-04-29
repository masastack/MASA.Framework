namespace Microsoft.Extensions.DependencyInjection;

public static partial class MasaServiceExtensions
{
    public static IServiceCollection AddMasaTracing(this IServiceCollection services, OpenTelemetryInstrumentationOptions options)
    {
        services.AddOpenTelemetryTracing((builder) =>
        {
            builder.SetSampler(new AlwaysOnSampler());

            if (options?.AspNetCoreInstrumentationOptions != null)
                builder.AddAspNetCoreInstrumentation(options.AspNetCoreInstrumentationOptions);

            if (options?.HttpClientInstrumentationOptions != null)
                builder.AddHttpClientInstrumentation(options.HttpClientInstrumentationOptions);

            if (options?.EntityFrameworkInstrumentationOptions != null)
                builder.AddEntityFrameworkCoreInstrumentation(options.EntityFrameworkInstrumentationOptions);

            if (options?.ElasticsearchClientInstrumentationOptions != null)
                builder.AddElasticsearchClientInstrumentation(options.ElasticsearchClientInstrumentationOptions);

            if (options?.StackExchangeRedisCallsInstrumentationOptions != null && options?.Connection != null)
                builder.AddRedisInstrumentation(options.Connection, options.StackExchangeRedisCallsInstrumentationOptions);

            options?.BuildTraceCallback?.Invoke(builder);
        });

        return services;
    }
}
