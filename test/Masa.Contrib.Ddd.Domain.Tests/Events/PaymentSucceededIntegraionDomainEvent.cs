namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record PaymentSucceededIntegraionDomainEvent(string OrderId, decimal Money, DateTime PayTime) : IntegrationDomainEvent
{
    public override string Topic { get; set; } = nameof(PaymentSucceededIntegraionDomainEvent);
}
