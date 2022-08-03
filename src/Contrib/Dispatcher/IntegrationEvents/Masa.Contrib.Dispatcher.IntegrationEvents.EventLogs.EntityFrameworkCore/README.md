[中](README.zh-CN.md) | EN

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore

> Provide support for sending IntegrationEvent

Example：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore
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