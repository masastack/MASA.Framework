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

//metrics
builder.Services.AddMasaMetrics(builder => {
    builder.AddOtlpExporter();
});

//trcaing
var ops = new OpenTelemetryInstrumentationOptions();
//api 排除swagger和healthy
ops.AspNetCoreInstrumentationOptions.AppendDefaultFilter(ops);
//blazor 移除blazor常用资源请求路径
//ops.AspNetCoreInstrumentationOptions.AppendBlazorFilter(ops);
ops.BuildTraceCallback = builder => {
    builder.AddOtlpExporter();
};
builder.Services.AddMasaTracing(ops);

//logging
builder.Logging.AddMasaOpenTelemetry(options =>
{
    var ops = ResourceBuilder.CreateDefault().AddService(serviceName: "servicename");
    options.SetResourceBuilder(ops);
    options.AddOtlpExporter();
});

```
