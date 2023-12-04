// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddMasaTracing(this IServiceCollection services,
        Action<TracerProviderBuilder> builderConfigure,
        Action<OpenTelemetryInstrumentationOptions>? configure = null)
    {
        services.AddOpenTelemetry().AddMasaTracing(services, builderConfigure, configure);
        return services;
    }

    internal static OpenTelemetryBuilder AddMasaTracing(this OpenTelemetryBuilder builder,
        IServiceCollection services,
        Action<TracerProviderBuilder> builderConfigure,
        Action<OpenTelemetryInstrumentationOptions>? openTelemetryInstrumentationOptions = null)
    {
        return builder.WithTracing(builder =>
        {
            builder.SetSampler(new AlwaysOnSampler());
            var option = services.BuildServiceProvider().GetService<OpenTelemetryInstrumentationOptions>();
            option ??= new OpenTelemetryInstrumentationOptions(services.BuildServiceProvider());
            openTelemetryInstrumentationOptions?.Invoke(option);

            if (option.AspNetCoreInstrumentationOptions != null)
                builder.AddAspNetCoreInstrumentation(option.AspNetCoreInstrumentationOptions);

            if (option.HttpClientInstrumentationOptions != null)
                builder.AddHttpClientInstrumentation(option.HttpClientInstrumentationOptions);

            if (option.EntityFrameworkInstrumentationOptions != null)
                builder.AddEntityFrameworkCoreInstrumentation(option.EntityFrameworkInstrumentationOptions);

            if (option.ElasticsearchClientInstrumentationOptions != null)
                builder.AddElasticsearchClientInstrumentation(option.ElasticsearchClientInstrumentationOptions);

            if (option.ConnectionMultiplexerOptions != null)
            {
                foreach (Delegate handle in option.ConnectionMultiplexerOptions.GetInvocationList())
                {
                    var obj = handle.DynamicInvoke();
                    builder.AddRedisInstrumentation((IConnectionMultiplexer)obj!, options =>
                    {
                        options.SetVerboseDatabaseStatements = true;
                        option.StackExchangeRedisInstrumentationOptions?.Invoke(options);
                    });
                }
            }

            builderConfigure?.Invoke(builder);
            option.BuildTraceCallback?.Invoke(builder);
        });
    }
}
