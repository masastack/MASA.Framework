// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static partial class MasaServiceExtensions
{
    public static IServiceCollection AddMasaMetrics(this IServiceCollection services, Action<MeterProviderBuilder>? configure = null)
    {
        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            configure?.Invoke(builder);
            builder.AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddMeter(HttpMetricProviders.Meter.Name);
        }).StartWithHost();
        return services;
    }
}
