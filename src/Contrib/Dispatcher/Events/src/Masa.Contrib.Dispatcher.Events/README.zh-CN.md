中 | [EN](README.md)

## EventBus

用例：

```c#
Install-Package Masa.Contrib.Dispatcher.Events
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
builder.Services.AddEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>)));
```

4. 支持Transaction

> 配合MASA.Contrib.Ddd.Domain.Repository.EF.Repository、UnitOfWork使用，当Event实现了ITransaction，会在执行Add、Update、Delete方法时自动开启事务，且在Handler全部执行后提交事务，当事务出现异常后，会自动回滚事务

##### 性能测试

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1023 (21H1/May2021Update)
11th Gen Intel Core i7-11700 2.50GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100-preview.4.22252.9
  [Host]     : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT DEBUG
  Job-MHJZJL : .NET 6.0.6 (6.0.622.26707), X64 RyuJIT

Runtime=.NET 6.0  IterationCount=100  RunStrategy=ColdStart

|                         Method |      Mean |     Error |      StdDev |   Median |      Min |         Max |
|------------------------------- |----------:|----------:|------------:|---------:|---------:|------------:|
|             SendCouponByDirect |  18.10 us |  47.19 us |   139.13 us | 3.600 us | 3.000 us |  1,395.4 us |
|           SendCouponByEventBus | 126.16 us | 374.20 us | 1,103.33 us | 9.950 us | 8.100 us | 11,043.7 us |
| AddShoppingCartByEventBusAsync | 124.80 us | 346.93 us | 1,022.94 us | 8.650 us | 6.500 us | 10,202.4 us |
|  AddShoppingCartByMediatRAsync | 110.57 us | 306.47 us |   903.64 us | 7.500 us | 5.300 us |  9,000.1 us |

##### 总结

IEventBus是事件总线的核心，配合Cqrs、Uow、Masa.Contrib.Ddd.Domain.Repository.EF使用，可实现自动执行SaveChange（启用UoW）与Commit（启用UoW且无关闭事务）操作，并支持出现异常后，回滚事务

> 问题1. 通过eventBus发布事件，Handler出错，但数据依然保存到数据库中，事务并未回滚

    > 1. 检查自定义事件或继承类，确保已经实现ITransaction
    > 2. 确认已使用UoW
    > 3. 确认UnitOfWork的UseTransaction属性为false
    > 4. 确认UnitOfWork的DisableRollbackOnFailure属性为true

> 问题2. 什么时候自动调用SaveChanges

    > 使用UoW且使用了MASA.Contrib.Ddd.Domain.Repository.EF，并且使用IRepository提供的Add、Update、Delete操作，通过EventBus发布事件，在执行EventHandler后会自动执行SaveChange

> 问题3. 如果在EventHandler中手动调用UoW的SaveChange方法保存，那框架还会自动保存吗？

    > 如果在EventHandler中手动调用了UoW的SaveChange方法保存，且之后并未再使用IRepository提供的Add、Update、Delete操作，则在EventHandler执行结束后不会二次执行SaveChange操作，但如果在手动调用UoW的SaveChange方法保存后又继续使用IRepository提供的Add、Update、Delete操作，则框架会再次调用SaveChange操作以确保数据保存成功

> 问题4. 为什么开启了异常重试却未执行重试？

    > 默认`UserFriendlyException`不支持重试，如果需要支持重试，则需要重新实现`IExceptionStrategyProvider`

> 问题5. EventBus是线程安全的吗？

    > EventBus不是线程安全的，如果多线程并发执行EventBus.PublishAsync()，则可能会出现数据未提交等异常