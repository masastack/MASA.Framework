[中](README.zh-CN.md) | EN

## Masa.Contrib.BasicAbility.Tsc

effect:

The tracking telemetry data adopts the Opentelemetry standard and integrates the OpenTelemetry SDK to report the telemetry tracking data to the OpenTelemetry-Collector collector, and then export and store the collected data Logging, Tracing, and Metrics through the Exporter

Example:


```C#
Install-Package Masa.Contrib.BasicAbility.Tsc
```

how to use:


```c#
// create and config ResourceBuilder instance
var resources = ResourceBuilder.CreateDefault();
resources.AddMasaService(new MasaObservableOptions
{
    ServiceName = "example.api"
});

//metrics
builder.Services.AddMasaMetrics(builder => {
    builder.SetResourceBuilder(resources);
    builder.AddOtlpExporter();
});

//trcaing
var ops = new OpenTelemetryInstrumentationOptions();
//api exclude swagger and healthy request
ops.AspNetCoreInstrumentationOptions.AppendDefaultFilter(ops);
//blazor exclude blazor resources request
//ops.AspNetCoreInstrumentationOptions.AppendBlazorFilter(ops);
ops.BuildTraceCallback = builder => {
    builder.SetResourceBuilder(resources);
    builder.AddOtlpExporter();
};
builder.Services.AddMasaTracing(ops);

//logging
builder.Logging.AddMasaOpenTelemetry(builder =>
{
    builder.SetResourceBuilder(resources);
    builder.AddOtlpExporter();
});

```
