中 | [EN](README.md)

## MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF

> 为发送IntegrationEvent提供支持

用例：

```C#
Install-Package MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF
```

1. 使用EventLogs.EF

```C#
.AddDaprEventBus<IntegrationEventLogService>(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> 提示：CustomDbContext需要继承IntegrationEventLogContext
