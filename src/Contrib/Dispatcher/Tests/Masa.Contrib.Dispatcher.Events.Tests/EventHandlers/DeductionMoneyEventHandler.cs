// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class DeductionMoneyEventHandler
{
    private readonly ILogger<DeductionMoneyEventHandler>? _logger;

    public DeductionMoneyEventHandler(ILogger<DeductionMoneyEventHandler>? logger = null)
    {
        _logger = logger;
    }

    [EventHandler]
    public async Task DeductionMoneyHandler(IEventBus eventBus, DeductionMoneyEvent @event)
    {
        // TODO: The simulated deduction is successful

        _logger?.LogInformation("deduct account balance {event}", @event.ToString());

        var increaseMoneyEvent = new IncreaseMoneyEvent()
        {
            Account = @event.PayeeAccount,
            TransferAccount = @event.Account,
            Money = @event.Money
        };
        await eventBus.PublishAsync(increaseMoneyEvent);
    }
}
