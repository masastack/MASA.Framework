中 | [EN](README.md)

## IntegrationEventBus

用例：

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents //使用跨进程事件
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //例如使用dapr提供pub、sub能力，也可自行选择其他实现
Install-Package Masa.Contrib.Data.UoW.EF //使用工作单元
Install-Package Masa.Contrib.Data.EntityFrameworkCore.SqlServer // 使用SqlServer
```

1. 添加IIntegrationEventBus

1.1 指定本地消息服务

```C#
builder.Services
    .AddIntegrationEventBus<CustomIntegrationEventLogService>(options=>
    {
        options.UseDapr();//使用Dapr提供pub/sub能力，也可以自行选择其他的
        options.UseUoW<CatalogDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"))//使用工作单元，推荐使用;
    });
```

>  CustomIntegrationEventLogService（自定义本地消息服务）需继承IIntegrationEventLogService，并且构造函数中的参数必须支持从CI获取

1.2 使用提供的EF版的本地消息服务

安装`Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF`

``` C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF //记录跨进程消息日志
```

```C#
builder.Services
    .AddIntegrationEventBus(options=>
    {
        options.UseDapr();//使用Dapr提供pub/sub能力，也可以自行选择其他的
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

### 重试策略

```C#
builder.Services
    .AddIntegrationEventBus<IntegrationEventLogService>(options=>
    {
        options.UseDapr();//使用Dapr提供pub/sub能力，也可以自行选择其他的
        // options.MaxRetryTimes = 50;//最大重试次数, 默认：50
        // options.RetryBatchSize = 100;//单次重试事件数量, 用于从持久化数据源获取待重试事件, 默认100
        // options.FailedRetryInterval = 60;//持久化数据源重试停歇间隔, 默认60s
        // options.CleaningExpireInterval = 300;//清除已过期事件停歇间隔，单位：s, 默认 300s
        // options.ExpireDate = 24 * 3600;//过期时间，CreationTime + ExpireDate = 过期时间, 默认1天

        // options.LocalFailedRetryInterval = 3;//本地队列重试停歇间隔, 默认3s
        // options.CleaningLocalQueueExpireInterval = 60;//清除本地队列已过期事件停歇间隔，单位：s, 默认 60s
    });
```

重试分为本地队列重试以及从持久化数据源重试两种：

本地队列：

特点：
- 重试间隔短，支持秒级别重试间隔
- 从内存获取数据，速度更快
- 系统崩溃后，之前的本地队列不会重建，自动降级到持久化队列中重试任务

持久化数据源队列：

特点：

- 系统崩溃后，可以从db或者其他持久化源获取重试队列，确保事件100%重试
- 作为本地内存队列的降级方案，对db或者其他数据源压力更低

在单副本情况下，两种队列的任务仅会在单个队列中执行，不会存在两个队列同时执行的情况。
在多副本情况下，同一个任务可能会被多个副本所执行，虽然我们有做幂等，但为交付保证是 At Least Once，仍然有可能出现事件发布成功，但状态更改失败的情况，
此时事件可能会重发，我们建议任务执行者做好对跨事件的重试

> 目前还未支持标准化的Sub能力，暂时使用实现方原生的写法
