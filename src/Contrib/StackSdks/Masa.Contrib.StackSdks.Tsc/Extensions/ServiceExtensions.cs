// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.Tests")]

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    private const string DEFAULT_CLIENT_NAME = "masa.contrib.stacksdks.tsc";

    public static IServiceCollection AddTscClient(this IServiceCollection services, string tscServiceBaseUrl)
    {
        ArgumentNullException.ThrowIfNull(tscServiceBaseUrl);

        if (services.Any(service => service.ServiceType == typeof(ITscClient)))
            return services;

        services.AddCaller(DEFAULT_CLIENT_NAME, builder =>
        {
            builder.UseHttpClient(options =>
            {
                options.BaseAddress = tscServiceBaseUrl;
            });
        });

        services.AddSingleton<ITscClient>(serviceProvider =>
        {
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            return new TscClient(caller);
        });

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static IServiceCollection AddObservable(this IServiceCollection services,
        ILoggingBuilder loggingBuilder,
        IConfiguration configuration,
        bool isBlazor = false,
        bool isInterruptSignalRTracing = true)
    {
        return services.AddObservable(loggingBuilder,
            configuration.GetSection("Masa:Observable").Get<MasaObservableOptions>(),
            configuration.GetSection("Masa:Observable:OtlpUrl").Get<string>(),
            isBlazor,
            isInterruptSignalRTracing);
    }

    public static IServiceCollection AddObservable(this IServiceCollection services,
        ILoggingBuilder loggingBuilder,
        Func<MasaObservableOptions> optionsConfigure,
        Func<string>? otlpUrlConfigure = null,
        bool isBlazor = false,
        bool isInterruptSignalRTracing = true)
    {
        ArgumentNullException.ThrowIfNull(optionsConfigure);
        var options = optionsConfigure();
        var otlpUrl = otlpUrlConfigure?.Invoke() ?? string.Empty;
        return services.AddObservable(loggingBuilder, options, otlpUrl, isBlazor, isInterruptSignalRTracing);
    }

    public static IServiceCollection AddObservable(this IServiceCollection services,
        ILoggingBuilder loggingBuilder,
        MasaObservableOptions option,
        string? otlpUrl = null,
        bool isBlazor = false,
        bool isInterruptSignalRTracing = true)
    {
        ArgumentNullException.ThrowIfNull(option);
        var resources = ResourceBuilder.CreateDefault().AddMasaService(option);

        if (string.IsNullOrEmpty(option.ServiceInstanceId))
            option.ServiceInstanceId = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetValue<string>("HOSTNAME");

        Uri? uri = null;
        if (!string.IsNullOrEmpty(otlpUrl) && !Uri.TryCreate(otlpUrl, UriKind.Absolute, out uri))
            throw new UriFormatException($"{nameof(otlpUrl)}:{otlpUrl} is invalid url");

        loggingBuilder.AddMasaOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options =>
            {
                if (uri != null)
                    options.Endpoint = uri;
            });
        });

        services.AddMasaMetrics(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(options =>
            {
                if (uri != null)
                    options.Endpoint = uri;
            });
        });

        return services.AddMasaTracing(builder =>
        {
            if (isBlazor)
                builder.AspNetCoreInstrumentationOptions.AppendBlazorFilter(builder, isInterruptSignalRTracing);
            else
                builder.AspNetCoreInstrumentationOptions.AppendDefaultFilter(builder, isInterruptSignalRTracing);

            builder.BuildTraceCallback = options =>
            {
                options.SetResourceBuilder(resources);
                options.AddOtlpExporter(options =>
                {
                    if (uri != null)
                        options.Endpoint = uri;
                });
            };
        });
    }
}
