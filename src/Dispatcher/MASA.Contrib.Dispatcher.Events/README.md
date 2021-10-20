[中](README.zh-CN.md) | EN

## EventBus

Example：

```c#
Install-Package MASA.Contrib.Dispatcher.Events
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
builder.Services
	   .AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>))
```

4. Support Transaction

> Used in conjunction with Contracts.EF and UnitOfWork, when Event implements ITransaction, the transaction will be automatically opened after the first CUD is executed, and the transaction will be submitted after all Handlers are executed. When an exception occurs in the transaction, the transaction will be automatically rolled back.
