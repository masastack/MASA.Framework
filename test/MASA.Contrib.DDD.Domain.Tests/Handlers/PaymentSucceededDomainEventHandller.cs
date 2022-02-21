namespace MASA.Contrib.DDD.Domain.Tests.Handlers;

public class PaymentSucceededDomainEventHandller
{
    private readonly ILogger<PaymentSucceededDomainEventHandller>? _logger;

    public PaymentSucceededDomainEventHandller(ILogger<PaymentSucceededDomainEventHandller>? logger)
    {
        _logger = logger;
    }

    [EventHandler]
    public Task PaymentSucceeded(PaymentSucceededDomainEvent domainEvent)
    {
        _logger?.LogInformation("PaymentSucceeded: OrderId: {OrderId}",domainEvent.OrderId);
        domainEvent.Result = true;
        return Task.CompletedTask;
    }
}
