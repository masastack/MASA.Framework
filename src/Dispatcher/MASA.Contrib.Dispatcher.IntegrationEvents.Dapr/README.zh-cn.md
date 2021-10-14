## IntegrationEventBus

用例：

```C#
Install-Package MASA.Contrib.Dispatcher.IntegrationEvents.Dapr //发送跨进程消息
Install-Package MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF //记录跨进程消息日志
Install-Package MASA.Contrib.Data.Uow.EF //使用工作单元
```

1. 添加IIntegrationEventBus

```C#
builder.Services
    .AddDaprEventBus<IntegrationEventLogService>(options=>
    {
    	options.UseUoW<CatalogDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"))//使用工作单元，推荐使用
               .UseEventLog<CatalogDbContext>();
        )
    });
```

> CustomerDbContext 需要继承IntegrationEventLogContext

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
public class CustomDbContext : IntegrationEventLogContext
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