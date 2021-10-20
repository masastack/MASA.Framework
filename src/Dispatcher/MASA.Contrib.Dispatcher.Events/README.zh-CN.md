中 | [EN](README.md)

## EventBus

用例：

```c#
Install-Package MASA.Contrib.Dispatcher.Events
```

##### 基本用法：

1. 添加EventBus

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Services
                 .AddEventBus()
                 //TODO
```

2. 自定义Event

```C#
public class TransferEvent : Event
{
    public string Account { get; set; } = default!;

    public string ReceiveAccount { get; set; } = default!;

    public decimal Money{ get; set; }
}
```

3. 发送Event

```C#
IEventBus eventBus;//通过DI得到IEventBus
await eventBus.PublishAsync(new TransferEvent());//发送Event
```

4. 定义Handler

```C#
public class TransferHandler
{
    [EventHandler]
    public Task TransferAsync(TransferEvent @event)
    {
        //TODO 模拟转账业务
    }
}
```

或使用实现接口的方式：

```C#
public class TransferHandler : IEventHandler<TransferEvent>
{
    public Task HandleAsync(TransferEvent @event)
    {
        //TODO 模拟转账业务
    }
}
```

##### 高级用法：

1. Handler编排：

```C#
public class TransferHandler
{
    [EventHandler(1)]
    public Task CheckBalanceAsync(TransferEvent @event)
    {
        //TODO 模拟检查余额
    }

    [EventHandler(2)]
    public Task DeductionBalanceAsync(RegisterUserEvent @event)
    {
        //TODO 模拟扣减余额
    }
}
```

2. 支持Saga模式

假如扣减余额发送出错，则重试3次，如果仍然失败则校验余额是否扣减，确保无扣减后通知转账失败

```C#
public class TransferHandler
{
    [EventHandler(1)]
    public Task CheckBalanceAsync(TransferEvent @event)
    {
        //TODO 模拟检查余额
    }

    [EventHandler(1, FailureLevels.Ignore, false, true)]
    public Task NotificationTransferFailedAsync(TransferEvent @event)
    {
        //TODO 模拟通知转账失败
    }

    [EventHandler(2, FailureLevels.ThrowAndCancel, true, 3)]
    public Task DeductionBalanceAsync(TransferEvent @event)
    {
        //TODO 模拟扣减余额
        throw new Exception("扣减余额失败");
    }

    [EventHandler(2, FailureLevels.Ignore, false, true)]
    public Task CancelDeductionBalanceAsync(TransferEvent @event)
    {
        //TODO 幂等校验，确保余额未扣减
    }
}
```

> 执行顺序： CheckBalanceAsync -> DeductionBalanceAsync （执行1次，重试3次）-> CancelDeductionBalanceAsync -> NotificationTransferFailedAsync

或者使用实现接口的方式

```C#
public class TransferHandler : ISagaEventHandler<TransferEvent>
{
    [EventHandler(1, FailureLevels.ThrowAndCancel, true, 3)]
    public Task HandleAsync(TransferEvent @event)
    {
        //TODO 模拟检查余额扣减余额
    }

    [EventHandler(1, FailureLevels.Ignore, false, true)]
    public Task CancelAsync(TransferEvent @event)
    {
        //TODO 幂等校验并通知转账失败
    }
}
```

> 注意：
> Handler所在的方法仅支持一个参数
> Handler所在的方法返回类型仅支持Task或void两种类型
> Handler所在的类的构造函数的参数必须支持从DI中获取

3. 支持Middleware

   1. 自定义Middleware
```C#
public class LoggingMiddleware<TEvent>
    : IMiddleware<TEvent> where TEvent : notnull, IEvent
{
    private readonly ILogger<LoggingMiddleware<TEvent>> _logger;
    public LoggingMiddleware(ILogger<LoggingMiddleware<TEvent>> logger) => _logger = logger;

    public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        _logger.LogInformation("----- Handling command {EventName} ({@Event})", typeof(TEvent).FullName, @event);
         await next();
    }
}
```
   2. 启用自定义Middleware


```C#
builder.Services
	   .AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>))
```

4. 支持Transaction

> 配合Contracts.EF、UnitOfWork使用，当Event实现了ITransaction，会在执行第一次CUD后自动开启事务，且在Handler全部执行后提交事务，当事务出现异常后，会自动回滚事务
