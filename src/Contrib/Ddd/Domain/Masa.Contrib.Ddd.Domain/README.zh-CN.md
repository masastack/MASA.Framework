中 | [EN](README.md)

## Masa.Contrib.Ddd.Domain

提供基础设施，使得基于`领域驱动设计`的开发更容易实现

用例：

``` powershell
Install-Package Masa.Contrib.Ddd.Domain // 提供领域事件，支持事件入队与发布
Install-Package Masa.Contrib.Ddd.Domain.Repository.EFCore // 提供基于IRepository的默认实现，并支持自定义Repository的自动注入

Install-Package Masa.Contrib.Dispatcher.Events //提供进程内事件（本地事件），支持事件编排、Saga、中间件

Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.Dapr //提供基于Dapr的发件箱
Install-Package Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore //提供基于EFCore的本地消息表
Install-Package Masa.Contrib.Data.UoW.EFCore //提供工作单元
Install-Package Masa.Contrib.Data.EFCore.SqlServer //提供基于EFCore的SqlServer实现
```

### 入门

1. 注册领域事件

```C#
builder.Services
.AddDomainEventBus(options =>
{
    options.UseIntegrationEventBus(opt =>
    {
        opt.UseDapr();
        opt.UseEventLog<CustomDbContext>();//使用跨进程事件
    });
    options
        // .UseEventBus(eventBuilder => eventBuilder.UseMiddleware(typeof(ValidatorMiddleware<>))) // 使用进程内事件并使用中间件
        .UseEventBus()//使用进程内事件
        .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlServer("server=localhost;uid=sa;pwd=P@ssw0rd;database=idientity"))
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
