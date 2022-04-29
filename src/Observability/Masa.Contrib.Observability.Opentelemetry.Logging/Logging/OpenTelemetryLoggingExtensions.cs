namespace Microsoft.Extensions.Logging;

public static class OpenTelemetryLoggingExtensions
{
    public static ILoggingBuilder AddMasaOpenTelemetry(this ILoggingBuilder builder, Action<OpenTelemetryLoggerOptions> configure, Action<OtlpExporterOptions>? exportConfigure = null)
    {
        builder = builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            if (configure != null)
                configure.Invoke(options);
            options.AddOtlpExporter(exportConfigure);
        });

        return builder;
    }
}
