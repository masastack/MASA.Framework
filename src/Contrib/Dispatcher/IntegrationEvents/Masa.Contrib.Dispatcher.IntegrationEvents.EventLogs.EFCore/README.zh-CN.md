中 | [EN](README.md)

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore

> 为发送IntegrationEvent提供支持

用例：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore
```

1. 使用EventLogs.EF

```C#
.AddIntegrationEventBus(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> 提示：CustomDbContext需要继承MasaDbContext
