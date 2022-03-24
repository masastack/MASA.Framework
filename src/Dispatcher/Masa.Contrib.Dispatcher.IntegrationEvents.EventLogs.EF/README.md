[中](README.zh-CN.md) | EN

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF

> Provide support for sending IntegrationEvent

Example：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF
```

1. Add EventLogs.EF

```C#
.AddDaprEventBus<IntegrationEventLogService>(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> Tip: CustomDbContext needs to inherit MasaDbContext