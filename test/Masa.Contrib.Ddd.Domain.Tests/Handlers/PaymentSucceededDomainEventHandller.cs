// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Tests.Handlers;

public class PaymentSucceededDomainEventHandller
{
    private readonly ILogger<PaymentSucceededDomainEventHandller>? _logger;

    public PaymentSucceededDomainEventHandller(ILogger<PaymentSucceededDomainEventHandller>? logger = null)
    {
        _logger = logger;
    }

    [EventHandler]
    public Task PaymentSucceeded(PaymentSucceededDomainEvent domainEvent)
    {
        _logger?.LogInformation("PaymentSucceeded: OrderId: {OrderId}", domainEvent.OrderId);
        domainEvent.Result = true;
        return Task.CompletedTask;
    }
}
