// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Tests")]

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceExtensions
{
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

        Uri? uri = null;
        if (!string.IsNullOrEmpty(otlpUrl) && !Uri.TryCreate(otlpUrl, UriKind.Absolute, out uri))
            throw new UriFormatException($"{nameof(otlpUrl)}:{otlpUrl} is invalid url");

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddMasaService(option))
            .AddMasaTracing(services, builder => builder.AddOtlpExporter(options => options.Endpoint = uri),
            builder =>
            {
                if (isBlazor)
                    builder.AspNetCoreInstrumentationOptions.AppendBlazorFilter(builder, isInterruptSignalRTracing);
                else
                    builder.AspNetCoreInstrumentationOptions.AppendDefaultFilter(builder, isInterruptSignalRTracing);
            })
            .AddMasaMetrics(builder => builder.AddOtlpExporter(otlp => otlp.Endpoint = uri));

        var resources = ResourceBuilder.CreateDefault().AddMasaService(option);
        loggingBuilder.AddMasaOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resources);
            builder.AddOtlpExporter(otlp => otlp.Endpoint = uri);
        });

        return services;
    }
}
