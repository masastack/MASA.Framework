namespace MASA.Contrib.DDD.Domain.Tests.Events;

public record PaymentFailedIntegrationDomainEvent : IntegrationDomainEvent
{
    public string OrderId { get; set; }

    public override string Topic { get; set; } = nameof(PaymentFailedIntegrationDomainEvent);
}
