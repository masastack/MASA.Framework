// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static partial class MasaServiceExtensions
{
    public static IServiceCollection AddMasaTracing(this IServiceCollection services, Action<OpenTelemetryInstrumentationOptions>? configure = null)
    {
        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.SetSampler(new AlwaysOnSampler());
            var option = new OpenTelemetryInstrumentationOptions();
            if (configure != null)
                configure.Invoke(option);

            if (option.AspNetCoreInstrumentationOptions != null)
                builder.AddAspNetCoreInstrumentation(option.AspNetCoreInstrumentationOptions);

            if (option.HttpClientInstrumentationOptions != null)
                builder.AddHttpClientInstrumentation(option.HttpClientInstrumentationOptions);

            if (option.EntityFrameworkInstrumentationOptions != null)
                builder.AddEntityFrameworkCoreInstrumentation(option.EntityFrameworkInstrumentationOptions);

            if (option.ElasticsearchClientInstrumentationOptions != null)
                builder.AddElasticsearchClientInstrumentation(option.ElasticsearchClientInstrumentationOptions);

            if (option.StackExchangeRedisCallsInstrumentationOptions != null && option.Connection != null)
                builder.AddRedisInstrumentation(option.Connection, option.StackExchangeRedisCallsInstrumentationOptions);

            option.BuildTraceCallback?.Invoke(builder);

            option.SetLogger(services.BuildServiceProvider().GetRequiredService<ILogger<OpenTelemetryInstrumentationOptions>>());
        }).StartWithHost();

        return services;
    }
}
