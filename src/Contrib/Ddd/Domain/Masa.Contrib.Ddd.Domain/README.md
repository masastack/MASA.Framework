[中](README.zh-CN.md) | EN

## Masa.Contrib.Ddd.Domain

Provides infrastructure to make development based on Domain Driven Design easier to achieve

Example：

``` powershell
Install-Package Masa.Contrib.Ddd.Domain // Provide domain events, support event enqueue and release
Install-Package Masa.Contrib.Ddd.Domain.Repository.EFCore // Provides a default implementation based on IRepository and supports automatic injection of custom Repository

Install-Package Masa.Contrib.Dispatcher.Events //Provide in-process events (local events), support event orchestration, Saga, middleware

Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //Provide Dapr-based outbox
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore //Provide EFCore-based local message table
Install-Package Masa.Contrib.Data.UoW.EFCore //Provide unit of work
Install-Package Masa.Contrib.Data.EFCore.SqlServer //Provide EFCore-based SqlServer implementation
```

### Get Started

1. Add DomainEventBus

```C#
builder.Services
.AddDomainEventBus(options =>
{
    options.UseIntegrationEventBus(opt =>
    {
        opt.UseDapr();
        opt.UseEventLog<CustomDbContext>();//Use cross-process events
    });
    options
        // .UseEventBus(eventBuilder => eventBuilder.UseMiddleware(typeof(ValidatorMiddleware<>))) // Use in-process events and use middleware
        .UseEventBus() // Use in-process events
        .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=idientity"))
        .UseRepository<CustomDbContext>();//Use the EF version of Repository to achieve
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
