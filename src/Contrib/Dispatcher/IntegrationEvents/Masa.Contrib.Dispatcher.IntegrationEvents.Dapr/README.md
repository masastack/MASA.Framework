[中](README.zh-CN.md) | EN

## IntegrationEvents.Dapr

Example:

``` powershell
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents //Use cross-process
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //Send cross-process messages
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore //Record cross-process message logs
Install-Package Masa.Contrib.Data.UoW.EFCore //Use UnitOfWork
Install-Package Masa.Contrib.Data.EFCore.SqlServer // Use SqlServer
```

### Get Started

1. Add IIntegrationEventBus

```C#
builder.Services
    .AddIntegrationEventBus<IntegrationEventLogService>(options=>
    {
        options.UseDapr();
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

> Implement pub/sub capability based on Dapr