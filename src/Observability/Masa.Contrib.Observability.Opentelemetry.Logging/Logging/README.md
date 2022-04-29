[中](README.zh-CN.md) | EN

## Opentelemetry.Logging

Example：

1. loggingadd project support，added based on [Openteletry specification](https://github.com/open-telemetry/opentelemetry-specification/tree/main/specification/common)contracted project support；
2. Added by default[OtlpExporter](https://github.com/open-telemetry/opentelemetry-collector/tree/main/exporter/otlpexporter)export，And log data is stored in Elasticsearch by default. If you need to modify the data storage medium, please refer to the configuration to modify.


```C#
Install-Package Masa.Contrib.Observability.Opentelemetry.Logging
```

1. Project support：

```C#
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddMasaService(
        serviceName: "service.example1",
        serviceProjectId: "masa.stack.auth",
        serviceVersion: "1.0.0"
        ));
    options.AddConsoleExporter();
});

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World!");
    return "Hello World!";
});

app.Run();

partial class Program { }
```

After running, you can see the project configuration information just now in the console output：
```
info: Program[0]
      Hello World!
LogRecord.TraceId:            6678d6ff3b922948e0d6de47b1beaf80
LogRecord.SpanId:             3c04a4585635b5ec
LogRecord.Timestamp:          2022-04-24T06:36:13.2218113Z
LogRecord.EventId:            0
LogRecord.EventName:
LogRecord.CategoryName:       Program
LogRecord.LogLevel:           Information
LogRecord.TraceFlags:         None
LogRecord.State:              Hello World!
Resource associated with LogRecord:
    service.project.id: masa.stack.auth
    service.name: service.example1
    service.version: 1.0.0
    service.instance.id: 2871cd69-a78b-4169-8b72-b30fb17c17f3
```


2. OTLP Expoter Configration

```C#
builder.Logging.AddMasaOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddMasaService(
        serviceName: "service.example1",
        serviceProjectId: "masa.stack.auth",
        serviceVersion: "1.0.0"
        ));
}, exportConfigure => {
    exportConfigure.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
    exportConfigure.Endpoint = new Uri("http://localhost:4317");
});
```
Detailed configuration reference[Opentelemetry Exporters](https://opentelemetry.io/docs/collector/configuration/#exporters).