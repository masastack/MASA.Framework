[中](README.zh-CN.md) | EN

## EventBus

Example：

```c#
Install-Package Masa.Contrib.Dispatcher.Events
```

##### Basic usage：

1. Add EventBus

```c#
var builder = WebApplication.CreateBuilder(args);
var app = builder.Services
                 .AddEventBus()
                 //TODO
```

2. Custom Event

```C#
public class TransferEvent : Event
{
    public string Account { get; set; } = default!;

    public string ReceiveAccount { get; set; } = default!;

    public decimal Money{ get; set; }
}
```

3. Send Event

```C#
IEventBus eventBus;//Get IEventBus through DI
await eventBus.PublishAsync(new TransferEvent());//Send Event
```

4. Define Handler

```C#
public class TransferHandler
{
    [EventHandler]
    public Task TransferAsync(TransferEvent @event)
    {
        //TODO Simulated transfer business
    }
}
```

Or use the way to implement the interface:

```C#
public class TransferHandler : IEventHandler<TransferEvent>
{
    public Task HandleAsync(TransferEvent @event)
    {
        //TODO Simulated transfer business
    }
}
```

##### Advanced usage:

1. Handler arrangement:

```C#
public class TransferHandler
{
    [EventHandler(1)]
    public Task CheckBalanceAsync(TransferEvent @event)
    {
        //TODO Simulate check balance
    }

    [EventHandler(2)]
    public Task DeductionBalanceAsync(RegisterUserEvent @event)
    {
        //TODO Simulated deduction balance
    }
}
```

2. Support Saga mode

If there is an error in sending the deducted balance, try again 3 times. If it still fails, check whether the balance is deducted and ensure that there is no deduction and notify the transfer failure

```C#
public class TransferHandler
{
    [EventHandler(1)]
    public Task CheckBalanceAsync(TransferEvent @event)
    {
        //TODO Simulate check balance
    }

    [EventHandler(1, FailureLevels.Ignore, false, true)]
    public Task NotificationTransferFailedAsync(TransferEvent @event)
    {
        //TODO Simulation notification transfer failed
    }

    [EventHandler(2, FailureLevels.ThrowAndCancel, true, 3)]
    public Task DeductionBalanceAsync(TransferEvent @event)
    {
        //TODO Simulated deduction balance
        throw new Exception("Failed to deduct balance");
    }

    [EventHandler(2, FailureLevels.Ignore, false, true)]
    public Task CancelDeductionBalanceAsync(TransferEvent @event)
    {
        //TODO Idempotent check to ensure that the balance has not been deducted
    }
}
```

> Execution order: CheckBalanceAsync -> DeductionBalanceAsync (execute 1 time, retry 3 times) -> CancelDeductionBalanceAsync -> NotificationTransferFailedAsync

Or use the way to implement the interface

```C#
public class TransferHandler : ISagaEventHandler<TransferEvent>
{
    [EventHandler(1, FailureLevels.ThrowAndCancel, true, 3)]
    public Task HandleAsync(TransferEvent @event)
    {
        //TODO Simulate check balance deduction balance
    }

    [EventHandler(1, FailureLevels.Ignore, false, true)]
    public Task CancelAsync(TransferEvent @event)
    {
        //TODO Idempotent verification and notification of transfer failure
    }
}
```

> Tip:
> The method where the Handler is located only supports one parameter
> The return type of the method where the Handler is located only supports Task or void two types
> The parameters of the constructor of the class where the Handler is located must support getting from DI

3. Support Middleware

   1. Custom Middleware
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
   2. Enable custom Middleware


```C#
builder.Services.AddEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(ValidatorMiddleware<>)));
```

4. Support Transaction

> Used in conjunction with Contracts.EF and UnitOfWork, when Event implements ITransaction, the transaction will be automatically opened after the first CUD is executed, and the transaction will be submitted after all Handlers are executed. When an exception occurs in the transaction, the transaction will be automatically rolled back.

##### Performance Testing

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

##### Summarize

IEventBus is the core of the event bus. It can be used with Cqrs, Uow, Masa.Contrib.Ddd.Domain.Repository.EF to automatically execute SaveChange (enable UoW) and Commit (enable UoW without closing transaction) operations after sending Command, And support to roll back the transaction after an exception occurs

> Question 1. Publishing events through eventBus, Handler error -> and handler throw exception

     > 1. Check custom events or inherited classes to make sure ITransaction is implemented
     > 2. Confirm that UoW is used
     > 3. Make sure the UseTransaction property of UnitOfWork is false
     > 4. Make sure that the DisableRollbackOnFailure property of UnitOfWork is true

> Question 2. Under what circumstances will SaveChange be automatically saved -> When auto call SaveChange?

    > Use UoW and Masa.Contrib.Ddd.Domain.Repository.EF, and use the Add, Update, Delete operations provided by IRepository, publish events through EventBus, and automatically execute SaveChange after executing EventHandler

> Question 3. If the SaveChange method of UoW is manually called in EventHandler to save, will the framework also save automatically?

    > If the SaveChange method of UoW is manually called in the EventHandler to save, and the Add, Update, and Delete operations provided by IRepository are not used afterward, the SaveChange operation will not be executed twice after the EventHandler execution ends, but if the UoW is manually called. After the SaveChange method is saved and continue to use the Add, Update, and Delete operations provided by IRepository, the framework will call the SaveChange operation again to ensure that the data is saved successfully.

> Question 4. Why is exception retry enabled but not executed?

     > The default `UserFriendlyException` does not support retries, if you need to support retries, you need to reimplement `IStrategyExceptionProvider`