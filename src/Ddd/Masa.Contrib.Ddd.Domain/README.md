[中](README.zh-CN.md) | EN

### DomainEventBus

Example：

```c#
Install-Package Masa.Contrib.Ddd.Domain
Install-Package Masa.Contrib.Ddd.Domain.Repository.EF

Install-Package Masa.Contrib.Dispatcher.Events

Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF
Install-Package Masa.Contrib.Data.UoW.EF
Install-Package Masa.Contrib.Data.EntityFrameworkCore.SqlServer
```

1. Add DomainEventBus

```C#
builder.Services
.AddDomainEventBus(options =>
{
    options.UseIntegrationEventBus<IntegrationEventLogService>(opt =>
    {
        opt.UseDapr();
        opt.UseEventLog<CustomizeDbContext>();//Use cross-process events
    });
    options
        // .UseEventBus(eventBuilder => eventBuilder.UseMiddleware(typeof(ValidatorMiddleware<>))) // Use in-process events and use middleware
        .UseEventBus() // Use in-process events
        .UseUoW<CustomizeDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=idientity"))
        .UseRepository<CustomizeDbContext>();//Use the EF version of Repository to achieve
})
```

2. Add DomainCommand

```C#
public class RegisterUserDomainCommand : DomainCommand
{
    public string User { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;
}
```
> DomainQuery refers to Query in Cqrs

3. Add Handler (in process)

```C#
public class UserHandler
{
    [EventHandler]
    public Task RegisterUserHandlerAsync(RegisterUserDomainCommand command)
    {
        //TODO Registered user business
    }
}
```

4. Send DomainCommand

```C#
IDomainEventBus eventBus;//Get IDomainEventBus through DI
await eventBus.PublishAsync(new RegisterUserDomainCommand());//Send DomainCommand
```

5. Define domain events

```C#
public class RegisterUserSucceededIntegrationEvent : IntegrationDomainEvent
{
    public override string Topic { get; set; } = nameof(RegisterUserSucceededIntegrationEvent);

    public string Account { get; set; } = default!;
}

public class RegisterUserSucceededEvent : DomainEvent
{
    public string Account { get; set; } = default!;
}
```

6. Define domain services

```C#
public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task RegisterSucceededAsync(string account)
    {
        await EventBus.Enqueue(new RegisterUserSucceededIntegrationEvent() { Account = account });
        await EventBus.Enqueue(new RegisterUserSucceededEvent() { Account = account });
        await EventBus.PublishQueueAsync();
    }
}
```

> DomainEvent (in-process) and IntegrationDomainEvent (cross-process) can be inherited as needed
>
> If you only need to send a domain event, you can directly use EventBus.PublishQueueAsync(new RegisterUserSucceededEvent())
>
> If you want to send in a unified way, you can send it through EventBus.Enqueue() and finally call EventBus.PublishQueueAsync()

Tip：

> 1. The use of DomainEventBus must require the implementation of IEventBus and IIntegrationEventBus and IUnitOfWork
> 2. EventBus only supports in-process scheduling, cross-process scheduling is not supported, and the sending order is consistent with the enqueue order, but the actual execution order is unknown

7. Cross-process event subscription

```C#
[Topic("pubsub", nameof(RegisterUserSucceededIntegrationEvent))]
public async Task RegisterUserSucceededHandlerAsync(RegisterUserSucceededIntegrationEvent @event)
{
    //todo
}
```
