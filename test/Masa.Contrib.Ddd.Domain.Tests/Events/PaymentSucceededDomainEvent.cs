namespace Masa.Contrib.Ddd.Domain.Tests.Events;

public record PaymentSucceededDomainEvent(string OrderId) : DomainEvent
{
    public bool Result { get; set; } = false;
}
