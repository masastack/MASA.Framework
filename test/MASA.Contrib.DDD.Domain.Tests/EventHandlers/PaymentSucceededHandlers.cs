namespace MASA.Contrib.DDD.Domain.Tests.EventHandlers;

public class PaymentSucceededHandlers : IEventHandler<PaymentSucceededDomainEvent>
{
    private readonly ILogger<PaymentSucceededHandlers> _logger;

    public PaymentSucceededHandlers(ILogger<PaymentSucceededHandlers> logger) => _logger = logger;

    public Task HandleAsync(PaymentSucceededDomainEvent @event)
    {
        _logger.LogInformation("Publishing PaymentSucceededDomainEvent {@Event} on {CreationTime}", @event, @event.CreationTime);
        return Task.CompletedTask;
    }
}
