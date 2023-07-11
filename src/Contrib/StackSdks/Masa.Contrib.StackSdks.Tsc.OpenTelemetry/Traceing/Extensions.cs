// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using OpenTelemetry;
using OpenTelemetry.Trace;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class Extensions
    {
        public static IServiceCollection AddMasaTracing(this IServiceCollection services, Action<TracerProviderBuilder> builderConfigure, Action<OpenTelemetryInstrumentationOptions>? configure = null)
        {
            services.AddOpenTelemetry().AddMasaTracing(services, builderConfigure, configure);
            return services;
        }

        internal static OpenTelemetryBuilder AddMasaTracing(this OpenTelemetryBuilder builder, IServiceCollection services, Action<TracerProviderBuilder> builderConfigure, Action<OpenTelemetryInstrumentationOptions>? configure = null)
        {
            return builder.WithTracing(builder =>
            {
                builder.SetSampler(new AlwaysOnSampler());
                var option = new OpenTelemetryInstrumentationOptions(services.BuildServiceProvider());
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
                builderConfigure?.Invoke(builder);
                option.BuildTraceCallback?.Invoke(builder);
            });
        }
    }
}
