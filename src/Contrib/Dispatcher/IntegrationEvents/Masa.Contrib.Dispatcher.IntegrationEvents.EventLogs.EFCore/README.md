[中](README.zh-CN.md) | EN

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore

> Provide support for sending IntegrationEvent

Example：

``` powershell
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore
```

### Get Started

1. Use local message table

```C#
.AddIntegrationEventBus(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> Tip: CustomDbContext needs to inherit MasaDbContext