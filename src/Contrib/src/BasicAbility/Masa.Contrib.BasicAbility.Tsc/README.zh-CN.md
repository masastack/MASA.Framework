中 | [EN](README.md)

## Masa.Contrib.BasicAbility.Tsc

作用：

追踪遥测数据采用Opentelemetry标准，并集成了OpenTelemetry SDK，将遥测追踪数据上报到OpenTelemetry-Collector采集器，
再将采集数据Logging、Tracing和Metrics经过Exporter导出存储

用例：

```C#
Install-Package Masa.Contrib.BasicAbility.Tsc
```

如何使用：

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
builder.Services.AddMasaTracing(options =>
{
    //api exclude swagger and healthy request
    options.AspNetCoreInstrumentationOptions.AppendDefaultFilter(options);

    //blazor exclude blazor resources request
    //options.AspNetCoreInstrumentationOptions.AppendBlazorFilter(options);

    options.BuildTraceCallback = builder =>
     {
         builder.SetResourceBuilder(resources);
         builder.AddOtlpExporter();
     };
});

//logging
builder.Logging.AddMasaOpenTelemetry(builder =>
{
    builder.SetResourceBuilder(resources);
    builder.AddOtlpExporter();
});

```
