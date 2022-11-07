// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class TransferEventHandler : ISagaEventHandler<TransferEvent>
{
    private readonly List<string> _blackAccount = new() { "roller", "thomas" };

    private readonly ILogger<TransferEventHandler>? _logger;

    public TransferEventHandler(ILogger<TransferEventHandler>? logger = null)
    {
        _logger = logger;
    }

    [EventHandler(EnableRetry = true, RetryTimes = 3)]
    public Task HandleAsync(TransferEvent @event)
    {
        if (_blackAccount.Contains(@event.Account))
        {
            throw new NotSupportedException("System error, please try again later");
        }
        _logger?.LogInformation("deduct account balance {event}", @event.ToString());
        return Task.CompletedTask;
    }

    [EventHandler(EnableRetry = true, RetryTimes = 3)]
    public Task CancelAsync(TransferEvent @event)
    {
        if (@event.Price > 1000000)
        {
            throw new NotSupportedException("Large transfer returns are not supported.");
        }
        else
        {
            return Task.CompletedTask;
        }
    }

    [EventHandler]
    public async Task DeductionMoneyHandler(IEventBus eventBus, DeductionMoneyEvent @event)
    {
        // TODO: The simulated deduction is successful
        
        _logger?.LogInformation("deduct account balance {event}", @event.ToString());

        IncreaseMoneyEvent increaseMoneyEvent = new IncreaseMoneyEvent()
        {
            Account = @event.PayeeAccount,
            TransferAccount = @event.Account,
            Money = @event.Money
        };
        await eventBus.PublishAsync(increaseMoneyEvent);
    }

    [EventHandler]
    public Task IncreaseMoneyHandler(IncreaseMoneyEvent @event)
    {
        // TODO: Succeeded in simulated increase
        return Task.CompletedTask;
    }
}

public class ReceiveTransferHandler
{
    private readonly List<string> _blackAccount = new List<string>() { "clark", "evan" };

    private readonly ILogger<ReceiveTransferHandler> _logger;

    public ReceiveTransferHandler(ILogger<ReceiveTransferHandler> logger) => _logger = logger;

    [EventHandler(EnableRetry = true, RetryTimes = 3, FailureLevels = FailureLevels.ThrowAndCancel)]
    public Task HandleAsync(TransferEvent @event)
    {
        if (_blackAccount.Contains(@event.OptAccount))
        {
            throw new NotSupportedException("System error, please try again later");
        }
        _logger.LogInformation("add opt account balance");
        return Task.CompletedTask;
    }
}
