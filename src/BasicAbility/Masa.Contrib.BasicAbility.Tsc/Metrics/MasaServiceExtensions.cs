// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class MasaServiceExtensions
    {
        public static IServiceCollection AddMasaMetrics(this IServiceCollection services, Action<MeterProviderBuilder> builderConfiger)
        {
            services.AddOpenTelemetryMetrics(builder =>
               {
                   builder
                   .AddRuntimeMetrics()
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation();

                   if (builderConfiger != null)
                       builderConfiger.Invoke(builder);
               }
            );
            return services;
        }
    }
}
