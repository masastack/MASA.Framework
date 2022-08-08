// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus;

public static class ServiceCollectionExtensions
{
    private const string PROMETHEUS_HTTP_CLIENT_NAME = "masa_stack_prometheus_client";

    public static IServiceCollection AddPrometheusClient(this IServiceCollection services, string url, int timeoutSeconds = 5)
    {
        ArgumentNullException.ThrowIfNull(url, nameof(url));

        if (services.Any(service => service.GetType() == typeof(IMasaPrometheusClient)))
            return services;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        if (timeoutSeconds <= 0)
            timeoutSeconds = 5;
        services.AddHttpClient(PROMETHEUS_HTTP_CLIENT_NAME, ops =>
        {
            ops.BaseAddress = new Uri(url);
            ops.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        services.AddScoped<IMasaPrometheusClient>(ServiceProvider =>
        {
            var client = ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(PROMETHEUS_HTTP_CLIENT_NAME);
            return new MasaPrometheusClient(client, jsonOptions, ServiceProvider.GetRequiredService<ILogger<MasaPrometheusClient>>());
        });
        return services;
    }
}
