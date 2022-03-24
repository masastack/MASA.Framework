中 | [EN](README.md)

### DomainEventBus

用例：

```c#
Install-Package Masa.Contrib.Ddd.Domain
Install-Package Masa.Contrib.Ddd.Domain.Repository.EF

Install-Package Masa.Contrib.Dispatcher.Events

Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF
Install-Package Masa.Contrib.Data.UoW.EF
Install-Package Masa.Utils.Data.EntityFrameworkCore.SqlServer
```

1. 添加DomainEventBus

```C#
builder.Services
.AddDomainEventBus(options =>
{
    // options.UseEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>)))//使用进程内事件并使用中间件
    options.UseEventBus()//使用进程内事件
        .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=idientity"))
        .UseDaprEventBus<IntegrationEventLogService>()///使用跨进程事件
        .UseEventLog<CustomDbContext>()
        .UseRepository<CustomDbContext>();//使用Repository的EF版实现
})
```

2. 添加DomainCommand

```C#
public class RegisterUserDomainCommand : DomainCommand
{
    public string User { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;
}
```
> DomainQuery参考Cqrs中的Query

3. 添加Handler（进程内）

```C#
public class UserHandler
{
    [EventHandler]
    public Task RegisterUserHandlerAsync(RegisterUserDomainCommand command)
    {
        //TODO 注册用户业务
    }
}
```

4. 发送DomainCommand

```C#
IDomainEventBus eventBus;//通过DI得到IDomainEventBus
await eventBus.PublishAsync(new RegisterUserDomainCommand());//发送DomainCommand
```

5. 定义领域事件

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

6. 定义领域服务

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

> 可根据需要继承DomainEvent（进程内）、IntegrationDomainEvent（跨进程）
>
> 如果只需要发送一个领域事件，则可以直接使用EventBus.PublishQueueAsync(new RegisterUserSucceededEvent())即可
>
> 如果希望统一发送，则可以通过EventBus.Enqueue()、最后调用EventBus.PublishQueueAsync()发送

提示：

> 1. 使用DomainEventBus必须要求实现IEventBus以及IIntegrationEventBus以及IUnitOfWork
> 2. EventBus只支持进程内编排、跨进程不支持编排，发送顺序与入队顺序一致，但实际执行顺序未知

7. 跨进程事件订阅

```C#
[Topic("pubsub", nameof(RegisterUserSucceededIntegrationEvent))]
public async Task RegisterUserSucceededHandlerAsync(RegisterUserSucceededIntegrationEvent @event)
{
    //todo
}
```
