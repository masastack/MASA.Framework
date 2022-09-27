中 | [EN](README.md)

## Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore

> 为发送IntegrationEvent提供支持

用例：

``` powershell
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore
```

### 入门

1. 使用本地消息表

```C#
.AddIntegrationEventBus(options =>
{
    options
        // TODO
        .UseEventLog<CustomDbContext>();
}
```

> 提示：CustomDbContext需要继承MasaDbContext
