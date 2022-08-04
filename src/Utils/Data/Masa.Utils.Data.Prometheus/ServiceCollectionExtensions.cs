// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus;

public static class ServiceCollectionExtensions
{
    private const string PROMETHEUS_HTTP_CLIENT_NAME = "prometheus_client_name";

    public static IServiceCollection AddPrometheusClient(this IServiceCollection services, string url)
    {
        ArgumentNullException.ThrowIfNull(url, nameof(url));

        if (services.Any(service => service.GetType() == typeof(IMasaPrometheusClient)))
            return services;

        services.AddCaller(builder =>
        {
            builder.UseHttpClient(options =>
            {
                options.BaseAddress = url;
                options.Name = PROMETHEUS_HTTP_CLIENT_NAME;
            });
        });

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        jsonOptions.Converters.Add(new JsonStringEnumConverter());

        services.AddScoped<IMasaPrometheusClient>(ServiceProvider =>
        {
            var caller = ServiceProvider.GetRequiredService<ICallerFactory>().CreateClient(PROMETHEUS_HTTP_CLIENT_NAME);
            return new MasaPrometheusClient(caller, jsonOptions);
        });
        return services;
    }
}
