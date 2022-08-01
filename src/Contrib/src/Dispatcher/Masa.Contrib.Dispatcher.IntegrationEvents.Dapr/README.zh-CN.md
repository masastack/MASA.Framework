中 | [EN](README.md)

## IntegrationEvents.Dapr

用例：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents //使用跨进程事件
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //通过dapr完成跨进程事件
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF //记录跨进程消息日志
Install-Package Masa.Contrib.Data.UoW.EF //使用工作单元
Install-Package Masa.Contrib.Data.EntityFrameworkCore.SqlServer // 使用SqlServer
```

1. 添加IIntegrationEventBus

```C#
builder.Services
    .AddIntegrationEventBus<IntegrationEventLogService>(options=>
    {
        options.UseDapr();
        options.UseUoW<CatalogDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"))//使用工作单元，推荐使用
               .UseEventLog<CatalogDbContext>();
    });
```

> CustomerDbContext 需要继承MasaDbContext

2. 自定义 IntegrationEvent

```C#
public class DemoIntegrationEvent : IntegrationEvent
{
    public override string Topic { get; set; } = nameof(DemoIntegrationEvent);//dapr topic name

    //todo 自定义属性参数
}
```

3. 自定义CustomDbContext

```C#
public class CustomDbContext : MasaDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {

    }
}
```

4. 发送 Event

```C#
IIntegrationEventBus eventBus;//通过DI得到IIntegrationEventBus
await eventBus.PublishAsync(new DemoIntegrationEvent());//发送跨进程事件
```

5. 订阅事件

```C#
[Topic("pubsub", nameof(DomeIntegrationEvent))]
public async Task DomeIntegrationEventHandleAsync(DomeIntegrationEvent @event)
{
    //todo
}
```

> 基于Dapr实现pub/sub能力