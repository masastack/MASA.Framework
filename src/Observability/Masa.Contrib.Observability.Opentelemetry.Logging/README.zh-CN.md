中 | [EN](README.md)

## Opentelemetry.Logging

用例：

1. logging添加项目支持，添加了基于 [Openteletry specification](https://github.com/open-telemetry/opentelemetry-specification/tree/main/specification/common)约定的项目支持；
2. 默认添加了[OtlpExporter](https://github.com/open-telemetry/opentelemetry-collector/tree/main/exporter/otlpexporter)导出，并且日志数据默认存储到Elasticsearch，如果需要修改数据存储介质，请参考配置进行修改。



```C#
Install-Package Masa.Contrib.Observability.Opentelemetry.Logging
```

1. 项目支持：

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

运行后可以在控制台输出中看到刚才项目配置信息：
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


2. OTLP Expoter配置

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
详细配置参考[Opentelemetry Exporters](https://opentelemetry.io/docs/collector/configuration/#exporters)。
