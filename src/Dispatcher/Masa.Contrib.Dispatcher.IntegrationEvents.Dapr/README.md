[中](README.zh-CN.md) | EN

## IntegrationEventBus

Example:

```C#
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //Send cross-process messages
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF //Record cross-process message logs
Install-Package Masa.Contrib.Data.UoW.EF //Use UnitOfWork
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer // Use SqlServer
```

1. Add IIntegrationEventBus

```C#
builder.Services
    .AddDaprEventBus<IntegrationEventLogService>(options=>
    {
        options.UseUoW<CatalogDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"))
               .UseEventLog<CatalogDbContext>();
    });
```

> CustomerDbContext needs to inherit MasaDbContext

2. Custom IntegrationEvent

```C#
public class DemoIntegrationEvent : IntegrationEvent
{
    public override string Topic { get; set; } = nameof(DemoIntegrationEvent);//dapr topic name

    //todo Custom attribute parameters
}
```

3. Custom CustomDbContext

```C#
public class CustomDbContext : MasaDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {

    }
}
```

4. Send Event

```C#
IIntegrationEventBus eventBus;//Get IIntegrationEventBus through DI
await eventBus.PublishAsync(new DemoIntegrationEvent());//Send cross-process events
```

5. Subscribe to events

```C#
[Topic("pubsub", nameof(DomeIntegrationEvent))]
public async Task DomeIntegrationEventHandleAsync(DomeIntegrationEvent @event)
{
    //todo
}
```

### retry policy

```C#
builder.Services
    .AddDaprEventBus<IntegrationEventLogService>(options=>
    {
        // options.MaxRetryTimes = 50;//Maximum number of retries, default: 50
        // options.RetryBatchSize = 100;//Number of single retry events, used to get retry events from persistent data source, default 100
        // options.FailedRetryInterval = 60;//Persistent data source retry pause interval, default 60s
        // options.CleaningExpireInterval = 300;//Clearing expired event pause interval, unit: s, default 300s
        // options.ExpireDate = 24 * 3600;//Expiration time, CreationTime + ExpireDate = Expiration time, default 1 day

        // options.LocalFailedRetryInterval = 3;//Local queue retry pause interval, default 3s
        // options.CleaningLocalQueueExpireInterval = 60;//Clearing local queue expired event pause interval, unit: s, default 60s
    });
```

Retry is divided into local queue retry and retry from persistent data source:

local queue:

Features:
- Short retry interval, support second-level retry interval
- Get data from memory, faster
- After the system crashes, the previous local queue will not be rebuilt, and will be automatically demoted to the persistent queue to retry the task

Persistent data source queue:

Features:

- After the system crashes, the retry queue can be obtained from db or other persistent sources to ensure 100% retry of events
- As a downgrade solution for local memory queues, lower pressure on db or other data sources

In the case of a single copy, the tasks of the two queues will only be executed in a single queue, and there will be no simultaneous execution of the two queues.
In the case of multiple copies, the same task may be executed by multiple copies. Although we have made idempotent, but the delivery guarantee is At Least Once, it is still possible that the event publishing is successful, but the state change fails.
At this point, the event may be re-sent. We recommend that the task executor retry across events.