[中](README.zh-CN.md) | EN

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore

> Provide support for sending IntegrationEvent

Example：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore
```

1. Add EventLogs.EF

```C#
.AddIntegrationEventBus(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> Tip: CustomDbContext needs to inherit MasaDbContext