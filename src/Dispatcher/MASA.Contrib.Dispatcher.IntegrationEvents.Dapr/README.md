## IntegrationEventBus

Example：

```C#
Install-Package MASA.Contrib.Dispatcher.IntegrationEvents.Dapr //Send cross-process messages
Install-Package MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF //Record cross-process message logs
Install-Package MASA.Contrib.Data.Uow.EF //Use UnitOfWork
```

1. Add IIntegrationEventBus

```C#
builder.Services
    .AddDaprEventBus<IntegrationEventLogService>(options=>
    {
    	options.UseUoW<CatalogDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=identity"))
               .UseEventLog<CatalogDbContext>();
        )
    });
```

> CustomerDbContext needs to inherit IntegrationEventLogContext

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
public class CustomDbContext : IntegrationEventLogContext
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